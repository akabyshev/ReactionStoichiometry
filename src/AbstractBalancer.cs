using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;
using ReactionStoichiometry.Extensions;

namespace ReactionStoichiometry;

internal abstract class AbstractBalancer<T> : IBalancer
{
    protected readonly List<string> details = new();

    protected readonly List<string> diagnostics = new();

    protected readonly List<string> fragments = new();
    protected readonly Matrix<double> matrix;
    protected int ReactantsCount { get; }

    protected Func<int, string> LabelFor => fragments.Count <= Program.LETTER_LABEL_THRESHOLD
        ? Helpers.LetterLabel
        : Helpers.GenericLabel;

    protected AbstractBalancer(string equation)
    {
        try
        {
            Outcome = "<FAIL>";
            Skeletal = equation.Replace(" ", "");
            ReactantsCount = Skeletal.Split('=')[0].Split('+').Length;

            var chargeSymbols = new[] { "Qn", "Qp" };
            var chargeParsingRules = new[]
            {
                new[] { "Qn", @"Qn(\d*)$", "{$1-}" },
                new[] { "Qp", @"Qp(\d*)$", "{$1+}" }
            };

            fragments.AddRange(Regex.Split(Skeletal, RegexPatterns.FRAGMENT_DIVIDERS));

            List<string> elements = new();
            elements.AddRange(Regex.Matches(Skeletal, RegexPatterns.ELEMENT_SYMBOL).Select(m => m.Value)
                .Concat(chargeSymbols).Distinct());
            elements.Add("{e}");

            matrix = Matrix<double>.Build.Dense(elements.Count, fragments.Count);
            for (var r = 0; r < elements.Count; r++)
            {
                Regex regex = new(RegexPatterns.ELEMENT_TEMPLATE.Replace("X", elements[r]));

                for (var c = 0; c < fragments.Count; c++)
                {
                    var fragment = Helpers.UnfoldFragment(fragments[c]);
                    matrix[r, c] += regex.Matches(fragment).Sum(match => double.Parse(match.Groups[1].Value));
                }
            }

            foreach (var chargeParsingRule in chargeParsingRules)
            {
                for (var i = 0; i < fragments.Count; i++)
                {
                    fragments[i] = Regex.Replace(fragments[i], chargeParsingRule[1],
                        chargeParsingRule[2]);
                }
            }

            var totalCharge = matrix.Row(elements.IndexOf("Qp")) - matrix.Row(elements.IndexOf("Qn"));
            matrix.SetRow(elements.IndexOf("{e}"), totalCharge);

            if (totalCharge.CountNonZeroes() == 0)
            {
                matrix = matrix.RemoveRow(elements.IndexOf("{e}"));
                elements.Remove("{e}");
            }

            matrix = matrix.RemoveRow(elements.IndexOf("Qn"));
            elements.Remove("Qn");
            matrix = matrix.RemoveRow(elements.IndexOf("Qp"));
            elements.Remove("Qp");

            details.AddRange(Helpers.PrettyPrintMatrix("Chemical composition matrix", matrix.ToArray(),
                Helpers.PrettyPrintDouble, fragments, elements));
            details.Add(
                $"RxC: {matrix.RowCount}x{matrix.ColumnCount}, rank = {matrix.Rank()}, nullity = {matrix.Nullity()}");
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
            diagnostics.Add($"This equation can't be balanced: {e.Message}");
        }
    }

    public string Outcome { get; protected set; }
    public string Skeletal { get; }
    public string Details => string.Join(Environment.NewLine, details);
    public string Diagnostics => string.Join(Environment.NewLine, diagnostics);

    protected abstract void Balance();
    protected abstract long[] ScaleToIntegers(T[] v);
    protected abstract string PrettyPrinter(T value);

    protected string GetEquationWithPlaceholders()
    {
        List<string> l = new();
        List<string> r = new();

        for (var i = 0; i < fragments.Count; i++)
        {
            var t = LabelFor(i) + Program.MULTIPLICATION_SYMBOL + fragments[i];
            (i < ReactantsCount ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }
}