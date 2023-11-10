namespace ReactionStoichiometry;

using System.Numerics;
using Rationals;

internal static class RationalArrayExtensions
{
    internal static BigInteger[] ScaleToIntegers(this Rational[] rationals)
    {
        var multiple = rationals.Select(selector: static r => r.Denominator).Aggregate(LeastCommonMultiple);
        var wholes = rationals.Select(selector: r => (r * multiple).CanonicalForm.Numerator).ToArray();
        var divisor = wholes.Aggregate(BigInteger.GreatestCommonDivisor);
        return wholes.Select(selector: r => r / divisor).ToArray();

        static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            if (a == 0 || b == 0)
                return 0;
            return BigInteger.Abs(a * b) / BigInteger.GreatestCommonDivisor(a, b);
        }
    }

    internal static Boolean IsIdentityMatrix(this Rational[,] me)
    {
        if (me.RowCount() != me.ColumnCount())
            return false;

        for (var r = 0; r < me.RowCount(); r++)
        {
            for (var c = 0; c < me.ColumnCount(); c++)
            {
                if (r == c && !me[r, c].IsOne)
                    return false;
                if (r != c && !me[r, c].IsZero)
                    return false;
            }
        }

        return true;
    }

    internal static Int32 RowCount(this Rational[,] me) => me.GetLength(dimension: 0);

    internal static Int32 ColumnCount(this Rational[,] me) => me.GetLength(dimension: 1);

    internal static Rational[] Row(this Rational[,] me, Int32 r)
    {
        var result = new Rational[me.ColumnCount()];
        for (var c = 0; c < me.ColumnCount(); c++)
            result[c] = me[r, c];

        return result;
    }

    internal static Rational[] Column(this Rational[,] me, Int32 c)
    {
        var result = new Rational[me.RowCount()];
        for (var r = 0; r < me.RowCount(); r++)
            result[r] = me[r, c];

        return result;
    }

    internal static String Readable(this Rational[,] me, String title, Func<Int32, String>? columnHeaders = null)
    {
        List<String> result = new() { $"[[{title}]]" };

        List<String> line = new();
        if (columnHeaders != null)
        {
            line.Add(String.Empty);
            line.AddRange(Enumerable.Range(start: 0, me.ColumnCount()).Select(columnHeaders));
            result.Add(String.Join(separator: '\t', line));
        }

        for (var r = 0; r < me.RowCount(); r++)
        {
            line.Clear();
            line.Add($"R#{r + 1}");

            for (var c = 0; c < me.ColumnCount(); c++)
                line.Add(me[r, c].ToString(format: "C"));

            result.Add(String.Join(separator: '\t', line));
        }

        return String.Join(Environment.NewLine, result);
    }
}
