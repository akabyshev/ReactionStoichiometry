using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class ReducedMatrixOfDouble : MatrixOfDouble
{
    private ReducedMatrixOfDouble(Matrix<double> matrix) : base(matrix)
    {
    }

    internal static ReducedMatrixOfDouble CreateInstance(Matrix<double> matrix)
    {
        var result = new ReducedMatrixOfDouble(matrix);
        result.Reduce();
        return result;
    }
}