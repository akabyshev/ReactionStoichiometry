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

        internal static BigInteger[] ScaleToIntegers(this Rational[] rationals)
        {
            var multiple = rationals.Select(selector: static r => r.Denominator).Aggregate(LeastCommonMultiple);
            var wholes = rationals.Select(selector: r => (r * multiple).CanonicalForm.Numerator).ToArray();
            var divisor = wholes.Aggregate(BigInteger.GreatestCommonDivisor);
            return wholes.Select(selector: r => divisor != 0 ? r / divisor : r).ToArray();
        }

        internal static List<Int32[]> GeneratePermutations(Int32 length, Int32 maxValue)
        {
            var permutations = new List<Int32[]>();
            Generate(permutations, new Int32[length], maxValue, index: 0);
            return permutations;

            static void Generate(ICollection<Int32[]> permutations, Int32[] current, Int32 maxValue, Int32 index)
            {
                if (index == current.Length)
                {
                    permutations.Add((Int32[]) current.Clone());
                    return;
                }

                for (var i = 1; i <= maxValue; i++)
                {
                    current[index] = i;
                    Generate(permutations, current, maxValue, index + 1);
                }
            }
        }
    }
}
