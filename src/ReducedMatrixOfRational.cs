namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

internal class ReducedMatrixOfRational : MatrixOfRational
{
    private ReducedMatrixOfRational(Matrix<Double> matrix) : base(matrix)
    {
    }

    internal static ReducedMatrixOfRational CreateInstance(Matrix<Double> matrix)
    {
        var result = new ReducedMatrixOfRational(matrix);
        result.Reduce();
        return result;
    }
}