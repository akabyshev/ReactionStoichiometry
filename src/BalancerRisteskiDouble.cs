namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

internal class BalancerRisteskiDouble : BalancerRisteskiGeneric<Double>
{
    public BalancerRisteskiDouble(String equation) : base(equation)
    {
    }

    protected override Int64[] ScaleToIntegers(Double[] v) => Utils.ScaleDoubles(v);

    protected override ReducedMatrixOfDouble GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = M.Append(Matrix<Double>.Build.Dense(M.RowCount, 1));
        return ReducedMatrixOfDouble.CreateInstance(augmentedMatrix);
    }

    protected override String PrettyPrinter(Double value) => Utils.PrettyPrintDouble(value);
}