namespace ReactionStoichiometry;

using System.Globalization;
using Extensions;
using MathNet.Numerics.LinearAlgebra;

internal class MatrixOfDouble : AbstractReducibleMatrix<Double>
{
    protected MatrixOfDouble(Matrix<Double> matrix) : base(matrix, x => x) =>
        Basics = new BasicOperations
                 {
                     Add = (a, b) => a + b,
                     Subtract = (a, b) => a - b,
                     Multiply = (a, b) => a * b,
                     Divide = (a, b) => a / b,
                     IsNonZero = DoubleExtensions.IsNonZero,
                     AsString = d => d.ToString(CultureInfo.InvariantCulture)
                 };
}