namespace ReactionStoichiometry;
using Rationals;

internal sealed class BalancerRisteskiRational : BalancerRisteski<Rational>
{
    public BalancerRisteskiRational(String equation) : base(equation, Utils.PrettyPrintRational, Utils.ScaleRationals)
    {
    }

    protected override SpecialMatrixReducedRational GetReducedAugmentedMatrix() => SpecialMatrixReducedRational.CreateInstance(M);
}