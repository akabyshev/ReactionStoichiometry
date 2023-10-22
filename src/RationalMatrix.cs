using MathNet.Numerics.LinearAlgebra;
using Rationals;
using System.Globalization;

namespace ReactionStoichiometry
{
    internal class RationalMatrix : AbstractReducibleMatrix<Rational>
    {
        protected RationalMatrix(Matrix<double> matrix) : base(matrix, x => Rational.ParseDecimal(x.ToString(CultureInfo.InvariantCulture)))
        {
            Basics = new BasicOperations
            {
                Add = Rational.Add,
                Subtract = Rational.Subtract,
                Multiply = Rational.Multiply,
                Divide = Rational.Divide,
                IsNonZero = (r => !r.IsZero),
                AsString = (r => r.ToString("C"))
            };
        }
    }
}