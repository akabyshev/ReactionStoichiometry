using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using ReactionStoichiometry.Extensions;

namespace ReactionStoichiometry;

internal abstract class AbstractBalancer<T> : ISpecialToString
{
    protected readonly List<string> Details = new();
    protected readonly List<string> Diagnostics = new();
    protected readonly List<string> Fragments = new();
    protected readonly Matrix<double> M;
    protected readonly int ReactantsCount;
    protected readonly string Skeletal;
    protected string Outcome = "<FAIL>";

    protected Func<int, string> LabelFor => Fragments.Count <= Program.LETTER_LABEL_THRESHOLD
        ? Utils.LetterLabel
        : Utils.GenericLabel;

    protected AbstractBalancer(string equation)
    {
        try
        {
            Skeletal = equation.Replace(" ", "");
            ReactantsCount = Skeletal.Split('=')[0].Split('+').Length;

            var chargeSymbols = new[] { "Qn", "Qp" };
            var chargeParsingRules = new[]
            {
                new[] { "Qn", @"Qn(\d*)$", "{$1-}" },
                new[] { "Qp", @"Qp(\d*)$", "{$1+}" }
            };

            Fragments.AddRange(Regex.Split(Skeletal, Parsing.FRAGMENT_DIVIDERS));

            List<string> elements = new();
            elements.AddRange(Regex.Matches(Skeletal, Parsing.ELEMENT_SYMBOL).Select(m => m.Value)
                .Concat(chargeSymbols).Distinct());
            elements.Add("{e}");

            M = Matrix<double>.Build.Dense(elements.Count, Fragments.Count);
            for (var r = 0; r < elements.Count; r++)
            {
                Regex regex = new(Parsing.ELEMENT_TEMPLATE.Replace("X", elements[r]));

                for (var c = 0; c < Fragments.Count; c++)
                {
                    var fragment = Parsing.UnfoldFragment(Fragments[c]);
                    M[r, c] += regex.Matches(fragment).Sum(match => double.Parse(match.Groups[1].Value));
                }
            }

            foreach (var chargeParsingRule in chargeParsingRules)
            {
                for (var i = 0; i < Fragments.Count; i++)
                {
                    Fragments[i] = Regex.Replace(Fragments[i], chargeParsingRule[1],
                        chargeParsingRule[2]);
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

            Details.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", M.ToArray(),
                Utils.PrettyPrintDouble, Fragments, elements));
            Details.Add(
                $"RxC: {M.RowCount}x{M.ColumnCount}, rank = {M.Rank()}, nullity = {M.Nullity()}");
        }
        catch (Exception e)
        {
            throw new ApplicationSpecificException($"Parsing failed: {e.Message}");
        }

        try
        {
            Balance();
        }
        catch (ApplicationSpecificException e)
        {
            Diagnostics.Add($"This equation can't be balanced: {e.Message}");
        }
    }

    protected abstract void Balance();
    protected abstract long[] ScaleToIntegers(T[] v);
    protected abstract string PrettyPrinter(T value);

    protected string GetEquationWithPlaceholders()
    {
        List<string> l = new();
        List<string> r = new();

        for (var i = 0; i < Fragments.Count; i++)
        {
            var t = LabelFor(i) + Program.MULTIPLICATION_SYMBOL + Fragments[i];
            (i < ReactantsCount ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }

    public override string ToString()
    {
        return ToString(ISpecialToString.OutputFormat.Plain);
    }

    public string ToString(ISpecialToString.OutputFormat format)
    {
        return format switch
        {
            ISpecialToString.OutputFormat.Plain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            ISpecialToString.OutputFormat.Html => Fill(OutputTemplateStrings.HTML_OUTPUT),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        string Fill(string template)
        {
            return template
                .Replace("%Skeletal%", Skeletal)
                .Replace("%Details%", string.Join(Environment.NewLine, Details))
                .Replace("%Outcome%", Outcome)
                .Replace("%Diagnostics%", string.Join(Environment.NewLine, Diagnostics));
        }
    }
}