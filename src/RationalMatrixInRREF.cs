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
}