﻿namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics;
using Rationals;

internal static class Utils
{
    public static IEnumerable<String> PrettyPrintMatrix<T>(String title,
                                                           in T[,] matrix,
                                                           Func<Int32, String>? columnHeaders = null,
                                                           Func<Int32, String>? rowHeaders = null)
    {
        Func<T, String> printer;
        if (typeof(T) == typeof(Double))
            printer = static v => PrettyPrintDouble((Double)(v as Object)!);
        else if (typeof(T) == typeof(Rational))
            printer = static v => PrettyPrintRational((Rational)(v as Object)!);
        else
            throw new NotImplementedException($"Printer is not implemented for type {typeof(T)}");

        List<String> result = new() { $"[[{title}]]" };

        List<String> line = new();
        if (columnHeaders != null)
        {
            line.Add(String.Empty);
            line.AddRange(Enumerable.Range(0, matrix.GetLength(1)).Select(columnHeaders));
            result.Add(String.Join('\t', line));
        }

        for (var r = 0; r < matrix.GetLength(0); r++)
        {
            line.Clear();
            line.Add(rowHeaders != null ? rowHeaders(r) : $"R#{r + 1}");

            for (var c = 0; c < matrix.GetLength(1); c++)
            {
                line.Add(printer(matrix[r, c]));
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

    public static String LetterLabel(Int32 n) => ((Char)('a' + n)).ToString();
    public static String GenericLabel(Int32 n) => 'x' + (n + 1).ToString("D2");

    public static BigInteger[] ScaleDoubles(IEnumerable<Double> doubles) =>
        ScaleRationals(doubles.Select(static d => Rational.Approximate(d, Program.GOOD_ENOUGH_DOUBLE_ZERO)).ToArray());

    public static BigInteger[] ScaleRationals(Rational[] rationals)
    {
        var multiple = rationals.Select(static r => r.Denominator).Aggregate(Euclid.LeastCommonMultiple);
        var wholes = rationals.Select(x => (x * multiple).CanonicalForm.Numerator).ToArray();
        var divisor = wholes.Aggregate(Euclid.GreatestCommonDivisor);
        return wholes.Select(x => x / divisor).ToArray();
    }

    public static Boolean IsNonZeroDouble(Double d) => Math.Abs(d) > Program.GOOD_ENOUGH_DOUBLE_ZERO;
}