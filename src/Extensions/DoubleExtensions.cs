namespace ReactionStoichiometry.Extensions;

internal static class DoubleExtensions
{
    public static double NonZeroAbsoluteMinimum(this IEnumerable<double> v)
    {
        var nonZeroAbsValues = v.Where(x => x != 0).Select(Math.Abs).ToList();

        if (!nonZeroAbsValues.Any())
            throw new ArgumentException("No non-zero values found, this should have never been called");

        return nonZeroAbsValues.Min();
    }

    public static int CountNonZeroes(this IEnumerable<double> v)
    {
        return v.Count(IsNonZero);
    }

    public static bool IsNonZero(this double d)
    {
        return Math.Abs(d) > Program.DOUBLE_PSEUDOZERO;
    }
}