namespace ReactionStoichiometry;

using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using Rationals;

internal sealed class SpecialMatrixReducedRational : SpecialMatrixReducible<Rational>
{
    private SpecialMatrixReducedRational(Matrix<Double> matrix) : base(matrix, static x => Rational.ParseDecimal(x.ToString(CultureInfo.InvariantCulture))) =>
        Basics = new BasicOperations
                 {
                     Add = Rational.Add,
                     Subtract = Rational.Subtract,
                     Multiply = Rational.Multiply,
                     Divide = Rational.Divide,
                     IsNonZero = static r => !r.IsZero,
                     AsString = static r => r.ToString("C")
                 };

    internal static SpecialMatrixReducedRational CreateInstance(Matrix<Double> matrix)
    {
        var result = new SpecialMatrixReducedRational(matrix);
        result.Reduce();
        return result;
    }
}