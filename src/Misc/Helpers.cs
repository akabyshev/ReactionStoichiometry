using System.Text.RegularExpressions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Rationals;
using ReactionStoichiometry.Extensions;

namespace ReactionStoichiometry;

internal static class Helpers
{
    public static string PrettyPrintDouble(double value)
    {
        return (value >= 0 ? " " : "") + value.ToString("0.###");
    }

    public static string PrettyPrintRational(Rational value)
    {
        return value.ToString("C");
    }

    public static IEnumerable<string> PrettyPrintMatrix<T>(string title, in T[,] matrix, Func<T, string> printer,
        List<string>? columnHeaders = null, List<string>? rowHeaders = null)
    {
        List<string> result = new() { $"[[{title}]]" };

        List<string> line = new();
        if (columnHeaders != null)
        {
            line.Add(string.Empty);
            line.AddRange(columnHeaders);
            result.Add(string.Join('\t', line));
        }

        for (var r = 0; r < matrix.GetLength(0); r++)
        {
            line.Clear();
            line.Add(rowHeaders != null ? rowHeaders[r] : $"R#{r + 1}");

            for (var c = 0; c < matrix.GetLength(1); c++)
            {
                line.Add(printer(matrix[r, c]));
            }

            result.Add(string.Join('\t', line));
        }

        return result;
    }

    public static string UnfoldFragment(in string fragment)
    {
        var result = fragment;

        {
            Regex regex = new(RegexPatterns.ELEMENT_NO_INDEX);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var element = match.Groups[1].Value;
                var rest = match.Groups[2].Value;
                result = regex.Replace(result, element + "1" + rest, 1);
            }
        }
        {
            Regex regex = new(RegexPatterns.CLOSING_PARENTHESIS_NO_INDEX);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;

                result = regex.Replace(result, ")1", 1);
            }
        }
        {
            Regex regex = new(RegexPatterns.INNERMOST_PARENTHESES_INDEXED);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var token = match.Groups[1].Value;
                var index = match.Groups[2].Value;

                var repeated = string.Join("", Enumerable.Repeat(token, int.Parse(index)));
                result = regex.Replace(result, repeated, 1);
            }
        }

        return result;
    }

    public static string LetterLabel(int n)
    {
        return ((char)('a' + n)).ToString();
    }

    public static string GenericLabel(int n)
    {
        return 'x' + (n + 1).ToString("D2");
    }

    public static long[] ScaleDoubles(double[] doubles)
    {
        try
        {
            return ScaleRationals(doubles.Select(x => Rational.Approximate(x, Program.DOUBLE_PSEUDOZERO)).ToArray());
        }
        catch (OverflowException)
        {
            var v = Vector<double>.Build.DenseOfArray(doubles);
            var wholes = v.Divide(v.NonZeroAbsoluteMinimum()).Divide(Program.DOUBLE_PSEUDOZERO).Select(d => (long)d)
                .ToArray();
            var gcd = wholes.Aggregate(Euclid.GreatestCommonDivisor);
            return wholes.Select(x => x / gcd).ToArray();
        }
    }

    public static long[] ScaleRationals(Rational[] rationals)
    {
        var multiple = rationals.Select(r => r.Denominator).Aggregate(Euclid.LeastCommonMultiple);
        var wholes = rationals.Select(x => (x * multiple).CanonicalForm.Numerator).ToArray();
        var divisor = wholes.Aggregate(Euclid.GreatestCommonDivisor);
        return wholes.Select(x => (long)(x / divisor)).ToArray();
    }
}