namespace ReactionStoichiometry;

internal class BalancerRisteskiRational : AbstractBalancerRisteski<Rationals.Rational>
{
    public BalancerRisteskiRational(String equation) : base(equation)
    {
    }

    protected override Int64[] ScaleToIntegers(Rationals.Rational[] v) => Utils.ScaleRationals(v);

    protected override ReducedMatrixOfRational GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = M.Append(MathNet.Numerics.LinearAlgebra.Matrix<Double>.Build.Dense(M.RowCount, 1));
        return ReducedMatrixOfRational.CreateInstance(augmentedMatrix);
    }

    protected override String PrettyPrinter(Rationals.Rational value) => Utils.PrettyPrintRational(value);
}