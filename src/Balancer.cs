namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal abstract class Balancer<T> : ISpecialToStringProvider, IChemicalEntityList
{
    private readonly String _statusMessage;
    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    protected readonly Matrix<Double> M;
    protected readonly Func<T[], BigInteger[]> ScaleToIntegers;

    protected Balancer(String equation, Func<T[], BigInteger[]> scale)
    {
        Equation = new ChemicalReactionEquation(equation);
        ScaleToIntegers = scale;

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

        try
        {
            Balance();
            _statusMessage = "OK";
        }
        catch (BalancerException e)
        {
            _statusMessage = "This equation can't be balanced: " + e.Message;
        }
    }

    protected abstract IEnumerable<String> Outcome { get; }

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => Equation.EntitiesCount;
    public String GetEntity(Int32 i) => Equation.GetEntity(i);
    #endregion

    #region ISpecialToStringProvider Members
    public String ToString(ISpecialToStringProvider.OutputFormat format)
    {
        return format switch
        {
            ISpecialToStringProvider.OutputFormat.Plain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            ISpecialToStringProvider.OutputFormat.Html => Fill(OutputTemplateStrings.HTML_OUTPUT),
            ISpecialToStringProvider.OutputFormat.OutcomeCommaSeparated => String.Join(',', Outcome),
            ISpecialToStringProvider.OutputFormat.OutcomeNewLineSeparated => String.Join(Environment.NewLine, Outcome),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", Equation.Skeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", ToString(ISpecialToStringProvider.OutputFormat.OutcomeNewLineSeparated))
                           .Replace("%Diagnostics%", _statusMessage);
        }
    }
    #endregion

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        Equation.AssembleEquationString(coefficients,
                                        static value => value != BigInteger.Zero,
                                        static value => value == 1 || value == -1 ? String.Empty : BigInteger.Abs(value) + Program.MULTIPLICATION_SYMBOL,
                                        static (_, value) => value < 0);

    protected abstract void Balance();
}