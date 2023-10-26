using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class BalancerRisteskiDouble : AbstractBalancerRisteski<double>
{
    public BalancerRisteskiDouble(string equation) : base(equation)
    {
    }

    protected override long[] ScaleToIntegers(double[] v)
    {
        return Utils.ScaleDoubles(v);
    }

    protected override ReducedMatrixOfDouble GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = M.Append(Matrix<double>.Build.Dense(M.RowCount, 1));
        return ReducedMatrixOfDouble.CreateInstance(augmentedMatrix);
    }

    protected override string PrettyPrinter(double value)
    {
        return Utils.PrettyPrintDouble(value);
    }
}