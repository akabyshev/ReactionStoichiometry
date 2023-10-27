namespace ReactionStoichiometry;

internal class MatrixOfDouble : AbstractReducibleMatrix<Double>
{
    protected MatrixOfDouble(MathNet.Numerics.LinearAlgebra.Matrix<Double> matrix) : base(matrix, x => x) =>
        Basics = new BasicOperations
                 {
                     Add = (a, b) => a + b,
                     Subtract = (a, b) => a - b,
                     Multiply = (a, b) => a * b,
                     Divide = (a, b) => a / b,
                     IsNonZero = ReactionStoichiometry.Extensions.DoubleExtensions.IsNonZero,
                     AsString = d => d.ToString(System.Globalization.CultureInfo.InvariantCulture)
                 };
}