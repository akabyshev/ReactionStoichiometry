using System.Diagnostics;
using System.Numerics;
using Rationals;


namespace ReactionStoichiometry
{
    internal static partial class RationalMatrixOperations
    {
        internal static void TrimAndGetCanonicalForms(ref Rational[,] matrix)
        {
            var indexLastRowToCopy = matrix.RowCount() - 1;
            while (indexLastRowToCopy >= 0 && matrix.Row(indexLastRowToCopy).All(predicate: static r => r == 0))
            {
                indexLastRowToCopy--;
            }

            if (indexLastRowToCopy == -1)
            {
                throw new InvalidOperationException(message: "All-zeroes matrix");
            }

            var newArray = new Rational[indexLastRowToCopy + 1, matrix.ColumnCount()];
            for (var r = 0; r < newArray.RowCount(); r++)
            {
                for (var c = 0; c < newArray.ColumnCount(); c++)
                {
                    newArray[r, c] = matrix[r, c].CanonicalForm;
                }
            }

            matrix = newArray;
        }

        // ReSharper disable once InconsistentNaming
        internal static Rational[,] GetRREF(this Rational[,] me)
        {
            var result = (Rational[,]) me.Clone();

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

            return result;
        }

        internal static Rational[,] GetInverse(Rational[,] matrix)
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

            var rref = augmentedMatrix.GetRREF();

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
            var multiple = rationals.Select(selector: static r => r.Denominator).Aggregate(LeastCommonMultiple);
            var wholes = rationals.Select(selector: r => (r * multiple).CanonicalForm.Numerator).ToArray();
            var divisor = wholes.Aggregate(BigInteger.GreatestCommonDivisor);
            return wholes.Select(selector: r => divisor != 0 ? r / divisor : r).ToArray();
        }

        internal static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
        {
            if (a == 0 || b == 0)
            {
                return 0;
            }
            return BigInteger.Abs(a * b) / BigInteger.GreatestCommonDivisor(a, b);
        }
    }
}
