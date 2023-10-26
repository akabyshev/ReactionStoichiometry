using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class ReducedMatrixOfRational : MatrixOfRational
{
    public ReducedMatrixOfRational(Matrix<double> matrix) : base(matrix)
    {
        Reduce();
    }
}