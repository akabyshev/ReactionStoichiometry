namespace ReactionStoichiometry.Extensions;

internal static class DoubleExtensions
{
    public static Boolean IsNonZero(this Double d) => Math.Abs(d) > Program.GOOD_ENOUGH_DOUBLE_ZERO;
}