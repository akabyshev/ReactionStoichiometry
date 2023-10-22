using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class DoubleMatrix : CoreMatrix<double>
{
    public DoubleMatrix(Matrix<double> matrix) : base(matrix, x => x)
    {
        Basics = new Basics_<double>
        {
            Add = ((a, b) => a + b),
            Subtract = ((a, b) => a - b),
            Multiply = ((a, b) => a * b),
            Divide = ((a, b) => a / b),
            IsNonZero = (d => Math.Abs(d) > Helpers.FP_TOLERANCE),
            AsString = (d => d.ToString())
        };
    }
}