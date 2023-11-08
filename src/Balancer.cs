namespace ReactionStoichiometry;

using System.Numerics;
using Properties;

public abstract class Balancer
{
    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    private String _statusMessage = String.Empty;

    protected Balancer(String equation) => Equation = new ChemicalReactionEquation(equation);

    protected abstract IEnumerable<String> Outcome { get; }
    public Int32 SubstancesCount => Equation.SubstancesCount;

    public virtual String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.DetailedPlain => Fill(OutputFormatTemplates.PLAIN_OUTPUT)
          , OutputFormat.DetailedHtml => Fill(OutputFormatTemplates.HTML_OUTPUT)
          , OutputFormat.OutcomeOnlyCommas => String.Join(',', Outcome)
          , OutputFormat.OutcomeOnlyNewLine => String.Join(Environment.NewLine, Outcome)
          , OutputFormat.VectorsNotation => $"Error: {format} not implemented by {GetType().Name}"
          , _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", Equation.Skeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", ToString(OutputFormat.OutcomeOnlyNewLine))
                           .Replace("%Diagnostics%", _statusMessage);
        }
    }

    public void Balance()
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

    public String GetSubstance(Int32 i) => Equation.GetSubstance(i);

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        Utils.AssembleEquationString(coefficients
                                   , static value => value != BigInteger.Zero
                                   , static value =>
                                         BigInteger.Abs(value) == 1 ? String.Empty : BigInteger.Abs(value) + Settings.Default.MULTIPLICATION_SYMBOL
                                   , GetSubstance
                                   , static value => value < 0);

    protected abstract void BalanceImplementation();
}

public enum OutputFormat
{
    DetailedPlain
  , DetailedHtml
  , OutcomeOnlyCommas
  , OutcomeOnlyNewLine
  , VectorsNotation
}
