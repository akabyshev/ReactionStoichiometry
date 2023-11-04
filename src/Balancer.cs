namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal abstract class Balancer : IChemicalEntityList
{
    #region OutputFormat enum
    internal enum OutputFormat
    {
        Plain,
        OutcomeCommaSeparated,
        OutcomeNewLineSeparated,
        Html
    }
    #endregion

    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    protected readonly Matrix<Double> M;
    private String _statusMessage = String.Empty;

    protected Balancer(String equation)
    {
        Equation = new ChemicalReactionEquation(equation);

        try
        {
            Equation.Parse(out M);
        }
        catch (Exception e)
        {
            throw new BalancerException($"Parsing failed: {e.Message}");
        }

        Details.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", M.ToArray(), GetEntity, Equation.GetElement));
        Details.Add($"RxC: {M.RowCount}x{M.ColumnCount}, rank = {M.Rank()}, nullity = {M.Nullity()}");
    }

    protected abstract IEnumerable<String> Outcome { get; }

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => Equation.EntitiesCount;
    public String GetEntity(Int32 i) => Equation.GetEntity(i);
    #endregion

    internal String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.Plain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            OutputFormat.Html => Fill(OutputTemplateStrings.HTML_OUTPUT),
            OutputFormat.OutcomeCommaSeparated => String.Join(',', Outcome),
            OutputFormat.OutcomeNewLineSeparated => String.Join(Environment.NewLine, Outcome),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", Equation.Skeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", ToString(OutputFormat.OutcomeNewLineSeparated))
                           .Replace("%Diagnostics%", _statusMessage);
        }
    }

    internal void Balance()
    {
        try
        {
            BalanceImplementation();
            _statusMessage = "OK";
        }
        catch (BalancerException e)
        {
            _statusMessage = "This equation can't be balanced: " + e.Message;
        }
    }

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        Equation.AssembleEquationString(coefficients,
                                        static value => value != BigInteger.Zero,
                                        static value => value == 1 || value == -1 ? String.Empty : BigInteger.Abs(value) + Program.MULTIPLICATION_SYMBOL,
                                        static (_, value) => value < 0);

    protected abstract void BalanceImplementation();
}