using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using ReactionStoichiometry.Extensions;

namespace ReactionStoichiometry;

internal class MatrixOfDouble : AbstractReducibleMatrix<double>
{
    protected MatrixOfDouble(Matrix<double> matrix) : base(matrix, x => x)
    {
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
}