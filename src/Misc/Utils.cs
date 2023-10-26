using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Rationals;
using ReactionStoichiometry.Extensions;

namespace ReactionStoichiometry;

internal static class Utils
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