namespace ReactionStoichiometry;

using System.Numerics;

internal abstract class Balancer : IChemicalEntityList
{
    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    private String _statusMessage = String.Empty;

    protected Balancer(String equation) => Equation = new ChemicalReactionEquation(equation);

    protected abstract IEnumerable<String> Outcome { get; }

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        Utils.AssembleEquationString(coefficients,
                                     static value => value != BigInteger.Zero,
                                     static value => value == 1 || value == -1 ? String.Empty : BigInteger.Abs(value) + Properties.Settings.Default.MULTIPLICATION_SYMBOL,
                                     GetEntity,
                                     static (_, value) => value < 0);

    protected abstract void BalanceImplementation();

    internal virtual String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.FullPlain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            OutputFormat.FullHtml => Fill(OutputTemplateStrings.HTML_OUTPUT),
            OutputFormat.OutcomeOnlyCommas => String.Join(',', Outcome),
            OutputFormat.OutcomeOnlyNewLine => String.Join(Environment.NewLine, Outcome),
            OutputFormat.OutcomeVectorNotation => $"Error: {format} not supported by {GetType().Name}",
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", Equation.Skeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", ToString(OutputFormat.OutcomeOnlyNewLine))
                           .Replace("%Diagnostics%", _statusMessage);
        }
    }

    internal void Balance()
    {
        Details.AddRange(Equation.MatrixAsStrings());

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
        FullPlain,
        OutcomeOnlyCommas,
        OutcomeOnlyNewLine,
        OutcomeVectorNotation,
        FullHtml
    }
    #endregion
}