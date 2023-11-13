using System.Diagnostics;
using System.Numerics;
using Rationals;


namespace ReactionStoichiometry
{
    internal static partial class RationalMatrixOperations
    {
        public static void TrimAndGetCanonicalForms(ref Rational[,] matrix)
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

        // todo: encapsulate to one-liner
        // ReSharper disable once InconsistentNaming
        public static void TurnIntoRREF(this Rational[,] me)
        {
            var leadColumnIndex = 0;
            for (var r = 0; r < me.RowCount(); r++)
            {
                if (leadColumnIndex >= me.ColumnCount())
                {
                    break;
                }

                var i = r;
                while (me[i, leadColumnIndex].IsZero)
                {
                    i++;

                    if (i != me.RowCount())
                    {
                        continue;
                    }

                    i = r;

                    if (leadColumnIndex >= me.ColumnCount() - 1)
                    {
                        break;
                    }

                    leadColumnIndex++;
                }

                for (var c = 0; c < me.ColumnCount(); c++)
                {
                    (me[r, c], me[i, c]) = (me[i, c], me[r, c]);
                }

                var div = me[r, leadColumnIndex];
                if (div != 0)
                {
                    for (var c = 0; c < me.ColumnCount(); c++)
                    {
                        me[r, c] = (me[r, c] / div).CanonicalForm;
                    }
                }

                Parallel.For(fromInclusive: 0
                           , me.RowCount()
                           , body: k =>
                                   {
                                       // ReSharper disable twice AccessToModifiedClosure
                                       if (k == r)
                                       {
                                           return;
                                       }
                                       var factor = me[k, leadColumnIndex];
                                       for (var c = 0; c < me.ColumnCount(); c++)
                                       {
                                           me[k, c] = (me[k, c] - factor * me[r, c]).CanonicalForm;
                                       }
                                   });
                leadColumnIndex++;
            }
        }

        public static Rational[,] GetInverse(Rational[,] matrix)
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

            augmentedMatrix.TurnIntoRREF();

            var leftHalf = new Rational[size, size];
            var rightHalf = new Rational[size, size];
            for (var r = 0; r < size; r++)
            {
                for (var c = 0; c < size; c++)
                {
                    leftHalf[r, c] = augmentedMatrix[r, c];
                    rightHalf[r, c] = augmentedMatrix[r, c + size];
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
                Debug.WriteLine(augmentedMatrix.Readable(nameof(augmentedMatrix)));
                throw;
            }
        }

        public static BigInteger[] ScaleToIntegers(this Rational[] rationals)
        {
            var multiple = rationals.Select(selector: static r => r.Denominator).Aggregate(LeastCommonMultiple);
            var wholes = rationals.Select(selector: r => (r * multiple).CanonicalForm.Numerator).ToArray();
            var divisor = wholes.Aggregate(BigInteger.GreatestCommonDivisor);
            return wholes.Select(selector: r => r / divisor).ToArray();

            static BigInteger LeastCommonMultiple(BigInteger a, BigInteger b)
            {
                if (a == 0 || b == 0)
                {
                    return 0;
                }
                return BigInteger.Abs(a * b) / BigInteger.GreatestCommonDivisor(a, b);
            }
        }
    }
}
