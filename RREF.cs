using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry
{
    internal class RationalMatrixInRREF : RationalMatrix
    {
        public RationalMatrixInRREF(Matrix<double> matrix) : base(matrix)
        {
            ReduceToTrimmedRREF();
        }
    }

    internal class DoubleMatrixInRREF : DoubleMatrix
    {
        public DoubleMatrixInRREF(Matrix<double> matrix) : base(matrix)
        {
            ReduceToTrimmedRREF();
        }
    }

    internal static class Extensions
    {
        public static double NonZeroAbsoluteMinimum(this Vector<double> v)
        {
            var nonZeroAbsValues = v.Where(x => x != 0).Select(Math.Abs);

            if (nonZeroAbsValues.Any())
            {
                return nonZeroAbsValues.Min();
            }
            else
            {
                throw new ArgumentException("No non-zero values in this vector");
            }
        }

        public static int CountNonZeroes(this Vector<double> v)
        {
            return v.Count(IsNonZero);
        }

        public static bool HasZeroDeterminant(this Matrix<double> m)
        {
            return Math.Abs(m.Determinant()) < Helpers.FP_TOLERANCE;
        }

        private static bool IsNonZero(this double d)
        {
            return Math.Abs(d) > Helpers.FP_TOLERANCE;
        }
    }
}