using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class BalancerRisteskiDouble : AbstractBalancerRisteski<double>
{
    public BalancerRisteskiDouble(string equation) : base(equation)
    {
    }

    protected override long[] ScaleToIntegers(double[] v)
    {
        return Helpers.ScaleDoubles(v);
    }

    protected override ReducedMatrixOfDouble GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
        return ReducedMatrixOfDouble.CreateInstance(augmentedMatrix);
    }

    protected override string PrettyPrinter(double value)
    {
        return Helpers.PrettyPrintDouble(value);
    }
}