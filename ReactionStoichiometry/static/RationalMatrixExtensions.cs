using System.Diagnostics;
using System.Numerics;
using Rationals;

namespace ReactionStoichiometry
{
    internal static class RationalMatrixExtensions
    {
        internal static Int32 RowCount(this Rational[,] me)
        {
            return me.GetLength(dimension: 0);
        }

        internal static Boolean IsIdentityMatrix(this Rational[,] me)
        {
            if (me.RowCount() != me.ColumnCount())
            {
                return false;
            }

            for (var r = 0; r < me.RowCount(); r++)
            {
                for (var c = 0; c < me.ColumnCount(); c++)
                {
                    if (r == c && !me[r, c].IsOne)
                    {
                        return false;
                    }
                    if (r != c && !me[r, c].IsZero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal static Int32 ColumnCount(this Rational[,] me)
        {
            return me.GetLength(dimension: 1);
        }

        internal static Rational[] Row(this Rational[,] me, Int32 r)
        {
            var result = new Rational[me.ColumnCount()];
            for (var c = 0; c < me.ColumnCount(); c++)
            {
                result[c] = me[r, c];
            }

            return result;
        }

        internal static Rational[] Column(this Rational[,] me, Int32 c)
        {
            var result = new Rational[me.RowCount()];
            for (var r = 0; r < me.RowCount(); r++)
            {
                result[r] = me[r, c];
            }

            return result;
        }

        internal static String Readable(this Rational[,] me, String title, Func<Int32, String>? rowHeaders = null, Func<Int32, String>? columnHeaders = null)
        {
            rowHeaders ??= static i => $"R{i + 1:D2}";

            List<String> result = new() { title + ":" };

            if (columnHeaders != null)
            {
                List<String> line = new() { String.Empty };
                line.AddRange(Enumerable.Range(start: 0, me.ColumnCount()).Select(columnHeaders));
                result.Add(String.Join(separator: '\t', line));
            }

            for (var r = 0; r < me.RowCount(); r++)
            {
                List<String> line = new() { rowHeaders(r) };
                line.AddRange(Enumerable.Range(start: 0, me.ColumnCount()).Select(selector: c => me[r, c].ToString()!));
                result.Add(String.Join(separator: '\t', line));
            }

            return String.Join(Environment.NewLine, result);
        }

        // ReSharper disable once InconsistentNaming
        internal static Rational[,] GetRREF(this Rational[,] me, Boolean trim)
        {
            var result = (Rational[,])me.Clone();

            var leadColumnIndex = 0;
            for (var r = 0; r < result.RowCount(); r++)
            {
                if (leadColumnIndex >= result.ColumnCount())
                {
                    break;
                }

                var i = r;
                while (result[i, leadColumnIndex].IsZero)
                {
                    i++;

                    if (i != result.RowCount())
                    {
                        continue;
                    }

                    i = r;

                    if (leadColumnIndex >= result.ColumnCount() - 1)
                    {
                        break;
                    }

                    leadColumnIndex++;
                }

                for (var c = 0; c < result.ColumnCount(); c++)
                {
                    (result[r, c], result[i, c]) = (result[i, c], result[r, c]);
                }

                var div = result[r, leadColumnIndex];
                if (div != 0)
                {
                    for (var c = 0; c < result.ColumnCount(); c++)
                    {
                        result[r, c] = (result[r, c] / div).CanonicalForm;
                    }
                }

                Parallel.For(fromInclusive: 0
                           , result.RowCount()
                           , body: k =>
                                   {
                                       // ReSharper disable twice AccessToModifiedClosure
                                       if (k == r)
                                       {
                                           return;
                                       }
                                       var factor = result[k, leadColumnIndex];
                                       for (var c = 0; c < result.ColumnCount(); c++)
                                       {
                                           result[k, c] = (result[k, c] - factor * result[r, c]).CanonicalForm;
                                       }
                                   });
                leadColumnIndex++;
            }

            if (trim)
            {
                Helpers.TrimAndGetCanonicalForms(ref result);
            }

            return result;
        }

        internal static Rational[,] GetInverse(this Rational[,] matrix)
        {
            var size = matrix.RowCount();
            if (size != matrix.ColumnCount())
            {
                throw new ArgumentException(message: "Non-square matrix");
            }

            var augmentedMatrix = new Rational[size, 2 * size];
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    augmentedMatrix[i, j] = matrix[i, j];
                    augmentedMatrix[i, j + size] = i == j ? 1 : 0;
                }
            }

            var rref = augmentedMatrix.GetRREF(trim: false);

            var leftHalf = new Rational[size, size];
            var rightHalf = new Rational[size, size];
            for (var r = 0; r < size; r++)
            {
                for (var c = 0; c < size; c++)
                {
                    leftHalf[r, c] = rref[r, c];
                    rightHalf[r, c] = rref[r, c + size];
                }
            }

            try
            {
                AppSpecificException.ThrowIf(!leftHalf.IsIdentityMatrix(), message: "Singular matrix");
                return rightHalf;
            }
            catch
            {
                Debug.WriteLine($"Exception at {nameof(GetInverse)}");
                Debug.WriteLine(rref.Readable(nameof(rref)));
                throw;
            }
        }

        internal static BigInteger[] ScaleToIntegers(this Rational[] rationals)
        {
            var multiple = rationals.Select(selector: static r => r.Denominator).Aggregate(Helpers.LeastCommonMultiple);
            var wholes = rationals.Select(selector: r => (r * multiple).CanonicalForm.Numerator).ToArray();
            var divisor = wholes.Aggregate(BigInteger.GreatestCommonDivisor);
            return wholes.Select(selector: r => divisor != 0 ? r / divisor : r).ToArray();
        }
    }
}
