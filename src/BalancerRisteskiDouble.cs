namespace ReactionStoichiometry;
internal sealed class BalancerRisteskiDouble : BalancerRisteski<Double>
{
    public BalancerRisteskiDouble(String equation) : base(equation, Utils.PrettyPrintDouble, Utils.ScaleDoubles)
    {
    }

    protected override SpecialMatrixReducedDouble GetReducedAugmentedMatrix() => SpecialMatrixReducedDouble.CreateInstance(M);
}