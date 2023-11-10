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
      , Vectors
    }
    #endregion

    private readonly List<String> _details = new();

    internal readonly ChemicalReactionEquation Equation;
    internal String FailureMessage { get; private set; }

    protected abstract void Balance();
    protected abstract IEnumerable<String> Outcome(); // todo: name?

    protected Balancer(String equation)
    {
        Equation = new ChemicalReactionEquation(equation);
        FailureMessage = String.Empty;
    }

    public virtual String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.DetailedPlain => Fill(OutputFormatTemplates.PLAIN_OUTPUT)
          , OutputFormat.DetailedHtml => Fill(OutputFormatTemplates.HTML_OUTPUT)
          , OutputFormat.OutcomeOnlyCommas => String.Join(separator: ',', Outcome())
          , OutputFormat.OutcomeOnlyNewLine => String.Join(Environment.NewLine, Outcome())
          , OutputFormat.Vectors => $"Error: {format} not implemented by {GetType().Name}"
          , _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace(oldValue: "%Skeletal%", Equation.Skeletal)
                           .Replace(oldValue: "%Details%", String.Join(Environment.NewLine, _details))
                           .Replace(oldValue: "%Outcome%", ToString(OutputFormat.OutcomeOnlyNewLine))
                           .Replace(oldValue: "%Diagnostics%", FailureMessage);
        }
    }

    public Boolean Run()
    {
        _details.Add(Equation.CCM.Readable(title: "Chemical composition matrix", columnHeaders: i => Equation.Substances[i]));
        _details.Add(String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                 , Equation.CCM.RowCount()
                                 , Equation.CCM.ColumnCount()
                                 , Equation.CompositionMatrixRank
                                 , Equation.CompositionMatrixNullity));
        _details.Add(Equation.REF.Readable(title: "REF"));

        try
        {
            Balance();
        }
        catch (AppSpecificException e)
        {
            FailureMessage = "This equation can't be balanced: " + e.Message;
            return false;
        }

        return true;
    }

    // ReSharper disable once ArgumentsStyleNamedExpression
    public String EquationWithPlaceholders() =>
        AssembleEquationString(Enumerable.Range(start: 0, Equation.Substances.Count).ToArray()
                             , omit: static _ => false
                             , adapter: LabelFor
                             , predicateGoesToRHS: static _ => false
                             , allowEmptyRHS: true);

    public String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        AssembleEquationString(coefficients
                             , omit: static value => value.IsZero
                             , adapter: static value => BigInteger.Abs(value) == 1 ? String.Empty : BigInteger.Abs(value).ToString()
                             , predicateGoesToRHS: static value => value > 0); // ReSharper disable twice InconsistentNaming

    private String AssembleEquationString<T>(IReadOnlyList<T> values
                                           , Func<T, Boolean> omit
                                           , Func<T, String> adapter
                                           , Func<T, Boolean> predicateGoesToRHS
                                           , Boolean allowEmptyRHS = false)
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < values.Count; i++)
        {
            if (omit(values[i]))
                continue;

            var token = adapter(values[i]);
            if (token != String.Empty)
                token += Settings.Default.MULTIPLICATION_SYMBOL;

            (predicateGoesToRHS(values[i]) ? r : l).Add(token + Equation.Substances[i]);
        }

        if (r.Count == 0 && allowEmptyRHS)
            r.Add(item: "0");

        if (l.Count == 0 || r.Count == 0)
            return "Invalid input";

        return String.Join(separator: " + ", l) + " = " + String.Join(separator: " + ", r);
    }

    internal String LabelFor(Int32 i) =>
        Equation.Substances.Count > Settings.Default.LETTER_LABEL_THRESHOLD ? 'x' + (i + 1).ToString(format: "D2") : ((Char)('a' + i)).ToString();
}
