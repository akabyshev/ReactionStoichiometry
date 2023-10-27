namespace ReactionStoichiometry;

internal abstract class AbstractBalancer<T> : ISpecialToString
{
    protected readonly List<String> Details = new();
    protected readonly List<String> Diagnostics = new();
    protected readonly List<String> Fragments = new();
    protected readonly MathNet.Numerics.LinearAlgebra.Matrix<Double> M;
    protected readonly Int32 ReactantsCount;
    protected readonly String Skeletal;
    protected String Outcome = "<FAIL>";

    protected Func<Int32, String> LabelFor => Fragments.Count <= Program.LETTER_LABEL_THRESHOLD ? Utils.LetterLabel : Utils.GenericLabel;

    protected AbstractBalancer(String equation)
    {
        try
        {
            Skeletal = equation.Replace(" ", "");
            ReactantsCount = Skeletal.Split('=')[0].Split('+').Length;

            var chargeSymbols = new[] { "Qn", "Qp" };
            var chargeParsingRules = new[] { new[] { "Qn", @"Qn(\d*)$", "{$1-}" }, new[] { "Qp", @"Qp(\d*)$", "{$1+}" } };

            Fragments.AddRange(System.Text.RegularExpressions.Regex.Split(Skeletal, Parsing.FRAGMENT_DIVIDERS));

            List<String> elements = new();
            elements.AddRange(System.Text.RegularExpressions.Regex.Matches(Skeletal, Parsing.ELEMENT_SYMBOL).Select(m => m.Value).Concat(chargeSymbols)
                                    .Distinct());
            elements.Add("{e}");

            M = MathNet.Numerics.LinearAlgebra.Matrix<Double>.Build.Dense(elements.Count, Fragments.Count);
            for (var r = 0; r < elements.Count; r++)
            {
                System.Text.RegularExpressions.Regex regex = new(Parsing.ELEMENT_TEMPLATE.Replace("X", elements[r]));

                for (var c = 0; c < Fragments.Count; c++)
                {
                    var fragment = Parsing.UnfoldFragment(Fragments[c]);
                    M[r, c] += regex.Matches(fragment).Sum(match => Double.Parse(match.Groups[1].Value));
                }
            }

            foreach (var chargeParsingRule in chargeParsingRules)
            {
                for (var i = 0; i < Fragments.Count; i++)
                {
                    Fragments[i] = System.Text.RegularExpressions.Regex.Replace(Fragments[i], chargeParsingRule[1], chargeParsingRule[2]);
                }
            }

            var totalCharge = M.Row(elements.IndexOf("Qp")) - M.Row(elements.IndexOf("Qn"));
            M.SetRow(elements.IndexOf("{e}"), totalCharge);

            if (ReactionStoichiometry.Extensions.DoubleExtensions.CountNonZeroes(totalCharge) == 0)
            {
                M = M.RemoveRow(elements.IndexOf("{e}"));
                elements.Remove("{e}");
            }

            M = M.RemoveRow(elements.IndexOf("Qn"));
            elements.Remove("Qn");
            M = M.RemoveRow(elements.IndexOf("Qp"));
            elements.Remove("Qp");

            Details.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", M.ToArray(), Utils.PrettyPrintDouble, Fragments, elements));
            Details.Add($"RxC: {M.RowCount}x{M.ColumnCount}, rank = {M.Rank()}, nullity = {M.Nullity()}");
        } catch (Exception e)
        {
            throw new ApplicationSpecificException($"Parsing failed: {e.Message}");
        }

        try
        {
            Balance();
        } catch (ApplicationSpecificException e)
        {
            Diagnostics.Add($"This equation can't be balanced: {e.Message}");
        }
    }

    public String ToString(ISpecialToString.OutputFormat format)
    {
        return format switch
        {
            ISpecialToString.OutputFormat.Plain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            ISpecialToString.OutputFormat.Html => Fill(OutputTemplateStrings.HTML_OUTPUT),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", Skeletal).Replace("%Details%", String.Join(Environment.NewLine, Details)).Replace("%Outcome%", Outcome)
                           .Replace("%Diagnostics%", String.Join(Environment.NewLine, Diagnostics));
        }
    }

    protected abstract void Balance();
    protected abstract Int64[] ScaleToIntegers(T[] v);
    protected abstract String PrettyPrinter(T value);

    protected String GetEquationWithPlaceholders()
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < Fragments.Count; i++)
        {
            var t = LabelFor(i) + Program.MULTIPLICATION_SYMBOL + Fragments[i];
            (i < ReactantsCount ? l : r).Add(t);
        }

        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    public override String ToString() => ToString(ISpecialToString.OutputFormat.Plain);
}