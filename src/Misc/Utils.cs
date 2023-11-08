namespace ReactionStoichiometry;

using System.Numerics;
using Rationals;

internal static class Utils
{
    internal static void AssertStringsAreEqual(String lhs, String rhs)
    {
        if (lhs != rhs) throw new Exception($"{lhs} is not equal to {rhs}");
    }

    internal static String LetterLabel(Int32 n) => ((Char)('a' + n)).ToString();
    internal static String GenericLabel(Int32 n) => 'x' + (n + 1).ToString("D2");

    internal static BigInteger[] ScaleRationals(Rational[] rationals)
    {
        var multiple = rationals.Select(static r => r.Denominator).Aggregate(LeastCommonMultiple);
        var wholes = rationals.Select(x => (x * multiple).CanonicalForm.Numerator).ToArray();
        var divisor = wholes.Aggregate(BigInteger.GreatestCommonDivisor);
        return wholes.Select(x => x / divisor).ToArray();

        static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            if (a == 0 || b == 0) return 0;
            return BigInteger.Abs(a * b) / BigInteger.GreatestCommonDivisor(a, b);
        }
    }

    internal static String AssembleEquationString<T>(T[] values
                                                   , Func<T, Boolean> filter
                                                   , Func<T, String> adapter
                                                   , Func<Int32, String> stringsSource
                                                   , Func<T, Boolean> predicateLeftHandSide
                                                   , Boolean allowEmptyRightSide = false)
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < values.Length; i++)
        {
            if (filter(values[i])) (predicateLeftHandSide(values[i]) ? l : r).Add(adapter(values[i]) + stringsSource(i));
        }

        if (r.Count == 0 && allowEmptyRightSide) r.Add("0");

        if (l.Count == 0 || r.Count == 0) return "Invalid input";

        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    internal static Rational[,] WithoutTrailingZeroRows(Rational[,] array)
    {
        var indexLastCopiedRow = array.GetLength(0) - 1;
        while (indexLastCopiedRow >= 0 && IsRowAllZeroes(indexLastCopiedRow))
        {
            indexLastCopiedRow--;
        }

        if (indexLastCopiedRow < 0) throw new InvalidOperationException("All-zeroes matrix");

        var result = new Rational[indexLastCopiedRow + 1, array.GetLength(1)];
        for (var r = 0; r < result.GetLength(0); r++)
        {
            for (var c = 0; c < result.GetLength(1); c++)
            {
                result[r, c] = array[r, c];
            }
        }

        return result;

        Boolean IsRowAllZeroes(Int32 r)
        {
            for (var c = 0; c < array.GetLength(1); c++)
            {
                if (!array[r, c].IsZero) return false;
            }
            return true;
        }
    }
}
