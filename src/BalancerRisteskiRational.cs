namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;
using Rationals;

internal class BalancerRisteskiRational : BalancerRisteski<Rational>
{
    public BalancerRisteskiRational(String equation) : base(equation)
    {
    }

    protected override Int64[] ScaleToIntegers(Rational[] v) => Utils.ScaleRationals(v);

    protected override SpecialMatrixReducedRational GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = M.Append(Matrix<Double>.Build.Dense(M.RowCount, 1));
        return SpecialMatrixReducedRational.CreateInstance(augmentedMatrix);
    }

    protected override String PrettyPrinter(Rational value) => Utils.PrettyPrintRational(value);
}