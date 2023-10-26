using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class ReducedMatrixOfRational : MatrixOfRational
{
    private ReducedMatrixOfRational(Matrix<double> matrix) : base(matrix)
    {
    }

    internal static ReducedMatrixOfRational CreateInstance(Matrix<double> matrix)
    {
        var result = new ReducedMatrixOfRational(matrix);
        result.Reduce();
        return result;
    }
}