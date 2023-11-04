namespace ReactionStoichiometry;

using System.Numerics;

internal abstract class Balancer : IChemicalEntityList
{
    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    private String _statusMessage = String.Empty;

    protected Balancer(String equation)
    {
        try
        {
            Equation = new ChemicalReactionEquation(equation);
            Details.AddRange(Equation.MatrixAsStrings());
        }
        catch (Exception e)
        {
            throw new BalancerException($"Parsing failed: {e.Message}");
        }
    }

    protected abstract IEnumerable<String> Outcome { get; }

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        Equation.AssembleEquationString(coefficients,
                                        static value => value != BigInteger.Zero,
                                        static value => value == 1 || value == -1 ? String.Empty : BigInteger.Abs(value) + Program.MULTIPLICATION_SYMBOL,
                                        static (_, value) => value < 0);

    protected abstract void BalanceImplementation();

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

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => Equation.EntitiesCount;
    public String GetEntity(Int32 i) => Equation.GetEntity(i);
    #endregion

    #region Nested type: OutputFormat
    internal enum OutputFormat
    {
        Plain,
        OutcomeCommaSeparated,
        OutcomeNewLineSeparated,
        Html
    }
    #endregion
}