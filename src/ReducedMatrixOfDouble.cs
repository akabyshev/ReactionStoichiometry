using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class ReducedMatrixOfDouble : MatrixOfDouble
{
    public ReducedMatrixOfDouble(Matrix<double> matrix) : base(matrix)
    {
        Reduce();
    }
}