namespace ReactionStoichiometry;

internal class ReducedMatrixOfRational : MatrixOfRational
{
    private ReducedMatrixOfRational(MathNet.Numerics.LinearAlgebra.Matrix<Double> matrix) : base(matrix)
    {
    }

    internal static ReducedMatrixOfRational CreateInstance(MathNet.Numerics.LinearAlgebra.Matrix<Double> matrix)
    {
        var result = new ReducedMatrixOfRational(matrix);
        result.Reduce();
        return result;
    }
}