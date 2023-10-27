namespace ReactionStoichiometry;

internal class ReducedMatrixOfDouble : MatrixOfDouble
{
    private ReducedMatrixOfDouble(MathNet.Numerics.LinearAlgebra.Matrix<Double> matrix) : base(matrix)
    {
    }

    internal static ReducedMatrixOfDouble CreateInstance(MathNet.Numerics.LinearAlgebra.Matrix<Double> matrix)
    {
        var result = new ReducedMatrixOfDouble(matrix);
        result.Reduce();
        return result;
    }
}