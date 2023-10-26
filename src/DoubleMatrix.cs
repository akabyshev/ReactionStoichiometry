using MathNet.Numerics.LinearAlgebra;
using System.Globalization;

namespace ReactionStoichiometry;

internal class DoubleMatrix : AbstractReducibleMatrix<double>
{
    protected DoubleMatrix(Matrix<double> matrix) : base(matrix, x => x)
    {
        Basics = new BasicOperations
        {
            Add = ((a, b) => a + b),
            Subtract = ((a, b) => a - b),
            Multiply = ((a, b) => a * b),
            Divide = ((a, b) => a / b),
            IsNonZero = Extensions.IsNonZero,
            AsString = (d => d.ToString(CultureInfo.InvariantCulture))
        };
    }
}