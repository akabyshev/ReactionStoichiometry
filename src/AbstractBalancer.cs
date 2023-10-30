namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using Extensions;
using MathNet.Numerics.LinearAlgebra;

internal abstract partial class AbstractBalancer<T> : ProtoBalancer
{
    protected readonly List<String> Details = new();
    protected readonly List<String> Diagnostics = new();
    protected readonly List<String> Fragments = new();
    protected readonly Matrix<Double> M;
    protected readonly Int32 ReactantsCount;
    internal Int32 FragmentsCount => Fragments.Count;

    internal Func<Int32, String> LabelFor => Fragments.Count <= Program.LETTER_LABEL_THRESHOLD ? Utils.LetterLabel : Utils.GenericLabel;

    protected AbstractBalancer(String equation) : base(equation.Replace(" ", ""))
    {
        try
        {
            ReactantsCount = OriginalSkeletal.Split('=')[0].Split('+').Length;

            var chargeSymbols = new[] { "Qn", "Qp" };
            var chargeParsingRules = new[] { new[] { "Qn", @"Qn(\d*)$", "{$1-}" }, new[] { "Qp", @"Qp(\d*)$", "{$1+}" } };

            Fragments.AddRange(Regex.Split(OriginalSkeletal, Parsing.FRAGMENT_DIVIDERS));

            List<String> elements = new();
            elements.AddRange(Regex.Matches(OriginalSkeletal, Parsing.ELEMENT_SYMBOL).Select(m => m.Value).Concat(chargeSymbols).Distinct());
            elements.Add("{e}");

            M = Matrix<Double>.Build.Dense(elements.Count, Fragments.Count);
            for (var r = 0; r < elements.Count; r++)
            {
                Regex regex = new(Parsing.ELEMENT_TEMPLATE.Replace("X", elements[r]));

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
                    Fragments[i] = Regex.Replace(Fragments[i], chargeParsingRule[1], chargeParsingRule[2]);
                }
            }

            var totalCharge = M.Row(elements.IndexOf("Qp")) - M.Row(elements.IndexOf("Qn"));
            M.SetRow(elements.IndexOf("{e}"), totalCharge);

            if (totalCharge.CountNonZeroes() == 0)
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
            throw new BalancerException($"Parsing failed: {e.Message}");
        }

        try
        {
            Balance();
        } catch (BalancerException e)
        {
            Diagnostics.Add($"This equation can't be balanced: {e.Message}");
        }
    }

    internal String Fragment(Int32 i) => Fragments[i];

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

    protected String GetEquationWithCoefficients(in Int64[] coefficients)
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < Fragments.Count; i++)
        {
            if (coefficients[i] == 0) continue;

            var value = Math.Abs(coefficients[i]);
            var t = (value == 1 ? "" : value + Program.MULTIPLICATION_SYMBOL) + Fragments[i];
            (coefficients[i] < 0 ? l : r).Add(t);
        }

        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    internal override String ToString(OutputFormat format)
    {
        return format switch
        {
            OutputFormat.Plain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            OutputFormat.Html => Fill(OutputTemplateStrings.HTML_OUTPUT),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", OriginalSkeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", Outcome)
                           .Replace("%Diagnostics%", String.Join(Environment.NewLine, Diagnostics));
        }
    }
}