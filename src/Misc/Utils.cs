namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics;
using Properties;
using Rationals;

internal static class Utils
{
    internal static IEnumerable<String> PrettyPrintMatrix<T>(String title, in T[,] array, Func<Int32, String>? columnHeaders = null)
    {
        Func<T, String> printer;
        if (typeof(T) == typeof(Double))
            printer = static v => PrettyPrintDouble((Double)(v as Object)!);
        else if (typeof(T) == typeof(Rational))
            printer = static v => PrettyPrintRational((Rational)(v as Object)!);
        else
            throw new NotImplementedException($"Not implemented for type {typeof(T)}");

        List<String> result = new() { $"[[{title}]]" };

        List<String> line = new();
        if (columnHeaders != null)
        {
            line.Add(String.Empty);
            line.AddRange(Enumerable.Range(0, array.GetLength(1)).Select(columnHeaders));
            result.Add(String.Join('\t', line));
        }

        for (var r = 0; r < array.GetLength(0); r++)
        {
            line.Clear();
            line.Add($"R#{r + 1}");

            for (var c = 0; c < array.GetLength(1); c++)
            {
                line.Add(printer(array[r, c]));
            }

            result.Add(String.Join('\t', line));
        }

        return result;

        static String PrettyPrintDouble(Double value)
        {
            return (value >= 0 ? " " : String.Empty) + value.ToString("0.###");
        }

        static String PrettyPrintRational(Rational value)
        {
            return value.ToString("C");
        }
    }

    internal static String LetterLabel(Int32 n) => ((Char)('a' + n)).ToString();
    internal static String GenericLabel(Int32 n) => 'x' + (n + 1).ToString("D2");

    internal static BigInteger[] ScaleDoubles(IEnumerable<Double> doubles) =>
        ScaleRationals(doubles.Select(static d => Rational.Approximate(d, Settings.Default.GOOD_ENOUGH_FLOAT_PRECISION)));

    internal static BigInteger[] ScaleRationals(IEnumerable<Rational> rationals)
    {
        var enumerable = rationals as Rational[] ?? rationals.ToArray();
        var multiple = enumerable.Select(static r => r.Denominator).Aggregate(Euclid.LeastCommonMultiple);
        var wholes = enumerable.Select(x => (x * multiple).CanonicalForm.Numerator).ToArray();
        var divisor = wholes.Aggregate(Euclid.GreatestCommonDivisor);
        return wholes.Select(x => x / divisor).ToArray();
    }

    internal static Boolean IsZeroDouble(Double d) => Math.Abs(d) < Settings.Default.GOOD_ENOUGH_FLOAT_PRECISION;

    internal static String AssembleEquationString<T>(T[] values,
                                                     Func<T, Boolean> filter,
                                                     Func<T, String> adapter,
                                                     Func<Int32, String> stringsSource,
                                                     Func<Int32, T, Boolean> predicateLeftHandSide)
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < values.Length; i++)
        {
            if (filter(values[i])) (predicateLeftHandSide(i, values[i]) ? l : r).Add(adapter(values[i]) + stringsSource(i));
        }

        if (l.Count == 0 || r.Count == 0) return "Invalid input";

        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    internal static T[,] WithoutTrailingZeroRows<T>(T[,] array, Func<T, Boolean> predicateIsZero)
    {
        var indexLastCopiedRow = array.GetLength(0) - 1;
        while (indexLastCopiedRow >= 0 && IsRowAllZeroes(indexLastCopiedRow))
        {
            indexLastCopiedRow--;
        }

        if (indexLastCopiedRow < 0) throw new InvalidOperationException("All-zeroes matrix");

        var result = new T[indexLastCopiedRow + 1, array.GetLength(1)];
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
                if (!predicateIsZero(array[r, c])) return false;
            }
            return true;
        }
    }
}