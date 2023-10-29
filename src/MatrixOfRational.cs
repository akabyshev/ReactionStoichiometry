namespace ReactionStoichiometry;

using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using Rationals;

internal class MatrixOfRational : AbstractReducibleMatrix<Rational>
{
    protected MatrixOfRational(Matrix<Double> matrix) : base(matrix, x => Rational.ParseDecimal(x.ToString(CultureInfo.InvariantCulture))) =>
        Basics = new BasicOperations
                 {
                     Add = Rational.Add,
                     Subtract = Rational.Subtract,
                     Multiply = Rational.Multiply,
                     Divide = Rational.Divide,
                     IsNonZero = r => !r.IsZero,
                     AsString = r => r.ToString("C")
                 };
}