using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Rationals;
using System.Text.RegularExpressions;

namespace ReactionStoichiometry
{
    internal static class Helpers
    {
        public const double FP_TOLERANCE = 1e-10;

        public static string PrettyPrintDouble(double value)
        {
            return (value >= 0 ? " " : "") + value.ToString("0.###");
        }

        public static string PrettyPrintRational(Rational value)
        {
            return value.ToString("C");
        }

        public static List<string> PrettyPrintMatrix<T>(string title, in T[,] matrix, Func<T, string> printer, List<string>? columnHeaders = null, List<string>? rowHeaders = null)
        {
            var result = new List<string>();
            result.Add($"[[{title}]]");

            List<string> current_line = new();
            if (columnHeaders != null)
            {
                current_line.Add(string.Empty);
                current_line.AddRange(columnHeaders);
                result.Add(string.Join('\t', current_line));
            }

            for (var i_r = 0; i_r < matrix.GetLength(0); i_r++)
            {
                current_line.Clear();
                if (rowHeaders != null)
                {
                    current_line.Add(rowHeaders[i_r]);
                }
                else
                {
                    current_line.Add($"R#{i_r + 1}");
                }

                for (var i_c = 0; i_c < matrix.GetLength(1); i_c++)
                {
                    current_line.Add(printer(matrix[i_r, i_c]));
                }

                result.Add(string.Join('\t', current_line));
            }

            return result;
        }

        public static string UnfoldFragment(in string fragment)
        {
            var result = fragment;

            {
                Regex regex = new(RegexPatterns.ElementNoIndex);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success) { break; }
                    var element = match.Groups[1].Value;
                    var nextchar = match.Groups[2].Value;
                    result = regex.Replace(result, element + "1" + nextchar, 1);
                }
            }
            {
                Regex regex = new(RegexPatterns.ClosingParenthesisNoIndex);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success) { break; }

                    result = regex.Replace(result, ")1", 1);
                }
            }
            {
                Regex regex = new(RegexPatterns.InnermostParenthesesIndexed);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success) { break; }
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
            return ((char) ('a' + n)).ToString();
        }

        public static string GenericLabel(int n)
        {
            return 'x' + (n + 1).ToString("D2");
        }

        public static long[] ScaleDoubles(double[] doubles)
        {
            try
            {
                return ScaleRationals(doubles.Select(x => Rational.Approximate(x, FP_TOLERANCE)).ToArray());
            }
            catch (OverflowException)
            {
                var v = Vector<double>.Build.DenseOfArray(doubles);
                var wholes = v.Divide(v.NonZeroAbsoluteMinimum()).Divide(FP_TOLERANCE).Select(d => (long) d).ToArray();
                var gcd = wholes.Aggregate(Euclid.GreatestCommonDivisor);
                return wholes.Select(x => x / gcd).ToArray();
            }
        }

        public static long[] ScaleRationals(Rational[] rationals)
        {
            long[] result;
            var multiple = rationals.Select(r => r.Denominator).Aggregate(Euclid.LeastCommonMultiple);
            var wholes = rationals.Select(x => (x * multiple).CanonicalForm.Numerator).ToArray();
            var divisor = wholes.Aggregate(Euclid.GreatestCommonDivisor);
            result = wholes.Select(x => (long) (x / divisor)).ToArray();
            return result;
        }

        internal static string SimpleStackedOutput<T>(AbstractBalancer<T> b)
        {
            return string.Join(
                Environment.NewLine,
                new List<string>() {
                "Skeletal:",
                b.Skeletal,
                string.Empty,
                "Details:",
                b.Details,
                string.Empty,
                "Outcome:",
                b.Outcome,
                string.Empty,
                "Diagnostics:",
                b.Diagnostics
                });
        }
    }
}
