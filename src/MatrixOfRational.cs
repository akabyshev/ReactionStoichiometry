namespace ReactionStoichiometry;

internal class MatrixOfRational : AbstractReducibleMatrix<Rationals.Rational>
{
    protected MatrixOfRational(MathNet.Numerics.LinearAlgebra.Matrix<Double> matrix) : base(matrix,
                                                                                            x => Rationals.Rational.ParseDecimal(
                                                                                                x.ToString(System.Globalization.CultureInfo
                                                                                                    .InvariantCulture))) =>
        Basics = new BasicOperations
                 {
                     Add = Rationals.Rational.Add,
                     Subtract = Rationals.Rational.Subtract,
                     Multiply = Rationals.Rational.Multiply,
                     Divide = Rationals.Rational.Divide,
                     IsNonZero = r => !r.IsZero,
                     AsString = r => r.ToString("C")
                 };
}