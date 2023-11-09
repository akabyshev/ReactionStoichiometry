namespace ReactionStoichiometry;

using System.Numerics;
using Properties;

public abstract class Balancer
{
    #region OutputFormat enum
    public enum OutputFormat
    {
        DetailedPlain
      , DetailedHtml
      , OutcomeOnlyCommas
      , OutcomeOnlyNewLine
      , VectorsNotation
    }
    #endregion

    protected readonly List<String> Details = new();

    protected readonly ChemicalReactionEquation Equation;
    private String _statusMessage = String.Empty;

    protected abstract IEnumerable<String> Outcome { get; }
    internal Int32 SubstancesCount => Equation.SubstancesCount;

    protected Balancer(String equation) => Equation = new ChemicalReactionEquation(equation);

    public virtual String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.DetailedPlain => Fill(OutputFormatTemplates.PLAIN_OUTPUT)
          , OutputFormat.DetailedHtml => Fill(OutputFormatTemplates.HTML_OUTPUT)
          , OutputFormat.OutcomeOnlyCommas => String.Join(separator: ',', Outcome)
          , OutputFormat.OutcomeOnlyNewLine => String.Join(Environment.NewLine, Outcome)
          , OutputFormat.VectorsNotation => $"Error: {format} not implemented by {GetType().Name}"
          , _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace(oldValue: "%Skeletal%", Equation.Skeletal)
                           .Replace(oldValue: "%Details%", String.Join(Environment.NewLine, Details))
                           .Replace(oldValue: "%Outcome%", ToString(OutputFormat.OutcomeOnlyNewLine))
                           .Replace(oldValue: "%Diagnostics%", _statusMessage);
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

    public String EquationWithPlaceholders() =>
        AssembleEquationString(Enumerable.Range(start: 0, SubstancesCount).Select(LabelFor).ToArray()
                             , filter: static _ => true
                             , adapter: static value => value + Settings.Default.MULTIPLICATION_SYMBOL
                             , GetSubstance
                             , predicateLeftHandSide: static _ => true
                             , allowEmptyRightSide: true);

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        AssembleEquationString(coefficients
                             , filter: static value => value != BigInteger.Zero
                             , adapter: static value =>
                                            BigInteger.Abs(value) == 1 ? String.Empty : BigInteger.Abs(value) + Settings.Default.MULTIPLICATION_SYMBOL
                             , GetSubstance
                             , predicateLeftHandSide: static value => value < 0);

    protected abstract void BalanceImplementation();

    protected static String AssembleEquationString<T>(T[] values
                                                    , Func<T, Boolean> filter
                                                    , Func<T, String> adapter
                                                    , Func<Int32, String> stringsSource
                                                    , Func<T, Boolean> predicateLeftHandSide
                                                    , Boolean allowEmptyRightSide = false)
    {
        // todo: meh...

        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < values.Length; i++)
        {
            if (filter(values[i])) (predicateLeftHandSide(values[i]) ? l : r).Add(adapter(values[i]) + stringsSource(i));
        }

        if (r.Count == 0 && allowEmptyRightSide) r.Add(item: "0");

        if (l.Count == 0 || r.Count == 0) return "Invalid input";

        return String.Join(separator: " + ", l) + " = " + String.Join(separator: " + ", r);
    }

    internal String LabelFor(Int32 i) =>
        SubstancesCount > Settings.Default.LETTER_LABEL_THRESHOLD ? 'x' + (i + 1).ToString(format: "D2") : ((Char)('a' + i)).ToString();

    internal String GetSubstance(Int32 i) => Equation.GetSubstance(i);
}
