using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class DoubleMatrixInRREF : DoubleMatrix
{
    public DoubleMatrixInRREF(Matrix<double> matrix) : base(matrix)
    {
        ReduceToTrimmedRREF();
    }
}