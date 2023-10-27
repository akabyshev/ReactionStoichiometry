namespace ReactionStoichiometry.Extensions;

internal static class DoubleExtensions
{
    public static Double NonZeroAbsoluteMinimum(this IEnumerable<Double> v)
    {
        var nonZeroAbsValues = v.Where(x => x != 0).Select(Math.Abs).ToList();

        if (!nonZeroAbsValues.Any()) throw new ArgumentException("No non-zero values found, this should have never been called");

        return nonZeroAbsValues.Min();
    }

    public static Int32 CountNonZeroes(this IEnumerable<Double> v) => v.Count(IsNonZero);

    public static Boolean IsNonZero(this Double d) => Math.Abs(d) > Program.DOUBLE_PSEUDOZERO;
}