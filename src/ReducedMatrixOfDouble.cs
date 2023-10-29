namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

internal class ReducedMatrixOfDouble : MatrixOfDouble
{
    private ReducedMatrixOfDouble(Matrix<Double> matrix) : base(matrix)
    {
    }

    internal static ReducedMatrixOfDouble CreateInstance(Matrix<Double> matrix)
    {
        var result = new ReducedMatrixOfDouble(matrix);
        result.Reduce();
        return result;
    }
}