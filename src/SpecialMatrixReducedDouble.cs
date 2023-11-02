namespace ReactionStoichiometry;

using System.Globalization;
using MathNet.Numerics.LinearAlgebra;

internal sealed class SpecialMatrixReducedDouble : SpecialMatrixReducible<Double>
{
    private SpecialMatrixReducedDouble(Matrix<Double> matrix) : base(matrix, static x => x) =>
        Basics = new BasicOperations
                 {
                     Add = static (d1, d2) => d1 + d2,
                     Subtract = static (d1, d2) => d1 - d2,
                     Multiply = static (d1, d2) => d1 * d2,
                     Divide = static (d1, d2) => d1 / d2,
                     IsZero = static d => !Utils.IsNonZeroDouble(d),
                     IsOne = static d => !Utils.IsNonZeroDouble(1.0d - d),
                     AsString = static d => d.ToString(CultureInfo.InvariantCulture)
                 };

    internal static SpecialMatrixReducedDouble CreateInstance(Matrix<Double> matrix)
    {
        var result = new SpecialMatrixReducedDouble(matrix);
        result.Reduce();
        return result;
    }
}