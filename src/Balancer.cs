namespace ReactionStoichiometry;

using System.Numerics;
using Properties;

internal abstract class Balancer : ISubstancesList
{
    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    private String _statusMessage = String.Empty;

    protected Balancer(String equation) => Equation = new ChemicalReactionEquation(equation);

    protected abstract IEnumerable<String> Outcome { get; }

    internal virtual String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.DetailedPlain => Fill(OutputFormatTemplates.PLAIN_OUTPUT),
            OutputFormat.DetailedHtml => Fill(OutputFormatTemplates.HTML_OUTPUT),
            OutputFormat.OutcomeOnlyCommas => String.Join(',', Outcome),
            OutputFormat.OutcomeOnlyNewLine => String.Join(Environment.NewLine, Outcome),
            OutputFormat.VectorsNotation => $"Error: {format} not implemented by {GetType().Name}",
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

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        Utils.AssembleEquationString(coefficients,
                                     static value => value != BigInteger.Zero,
                                     static value => value == 1 || value == -1 ? String.Empty : BigInteger.Abs(value) + Settings.Default.MULTIPLICATION_SYMBOL,
                                     GetSubstance,
                                     static (_, value) => value < 0);

    protected abstract void BalanceImplementation();

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

    #region ISubstancesList Members
    public Int32 SubstancesCount => Equation.SubstancesCount;
    public String GetSubstance(Int32 i) => Equation.GetSubstance(i);
    #endregion

    #region Nested type: OutputFormat
    internal enum OutputFormat
    {
        DetailedPlain,
        DetailedHtml,
        OutcomeOnlyCommas,
        OutcomeOnlyNewLine,
        VectorsNotation
    }
    #endregion
}