namespace ReactionStoichiometry;

using System.Globalization;
using Extensions;
using MathNet.Numerics.LinearAlgebra;

internal sealed class SpecialMatrixReducedDouble : SpecialMatrixReducible<Double>
{
    private SpecialMatrixReducedDouble(Matrix<Double> matrix) : base(matrix, x => x) =>
        Basics = new BasicOperations
                 {
                     Add = (a, b) => a + b,
                     Subtract = (a, b) => a - b,
                     Multiply = (a, b) => a * b,
                     Divide = (a, b) => a / b,
                     IsNonZero = DoubleExtensions.IsNonZero,
                     AsString = d => d.ToString(CultureInfo.InvariantCulture)
                 };

    internal static SpecialMatrixReducedDouble CreateInstance(Matrix<Double> matrix)
    {
        var result = new SpecialMatrixReducedDouble(matrix);
        result.Reduce();
        return result;
    }
}