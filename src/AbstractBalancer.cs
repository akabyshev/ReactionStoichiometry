using MathNet.Numerics.LinearAlgebra;
using System.Text.RegularExpressions;

namespace ReactionStoichiometry;

internal abstract class AbstractBalancer<T>
{
    public string Outcome { get; protected set; }
    public string Skeletal { get; }
    protected int ReactantsCount { get; }

    protected readonly List<string> fragments = new();
    protected readonly Matrix<double> matrix;

    protected const string MULTIPLICATION_SYMBOL = "·";
    protected Func<int, string> LabelFor => fragments.Count <= 7 ? Helpers.LetterLabel : Helpers.GenericLabel;

    protected readonly List<string> details = new();
    public string Details => string.Join(Environment.NewLine, details);

    protected readonly List<string> diagnostics = new();
    public string Diagnostics => string.Join(Environment.NewLine, diagnostics);

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
                new[] { "Qp", @"Qp(\d*)$", "{$1+}" },
            };

            fragments.AddRange(Regex.Split(Skeletal, RegexPatterns.FragmentDividers));

            List<string> elements = new();
            elements.AddRange(Regex.Matches(Skeletal, RegexPatterns.ElementSymbol).Select(m => m.Value)
                .Concat(chargeSymbols).Distinct());
            elements.Add("{e}");

            matrix = Matrix<double>.Build.Dense(elements.Count, fragments.Count);
            for (var i_r = 0; i_r < elements.Count; i_r++)
            {
                Regex regex = new(RegexPatterns.ElementTemplate.Replace("X", elements[i_r]));

                for (var i_c = 0; i_c < fragments.Count; i_c++)
                {
                    var plain_fragment = Helpers.UnfoldFragment(fragments[i_c]);
                    matrix[i_r, i_c] += regex.Matches(plain_fragment)
                        .Sum(match => double.Parse(match.Groups[1].Value));
                }
            }

            foreach (var chargeParsingRule in chargeParsingRules)
            {
                for (var i = 0; i < fragments.Count; i++)
                {
                    fragments[i] = Regex.Replace(fragments[i], pattern: chargeParsingRule[1],
                        replacement: chargeParsingRule[2]);
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

    protected abstract void Balance();
    protected abstract long[] ScaleVectorToIntegers(T[] v);
    protected abstract string PrettyPrinter(T value);
}