using System.Numerics;

using Rationals;

namespace ReactionStoichiometry
{
    internal static class Helpers
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
