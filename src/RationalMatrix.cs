using MathNet.Numerics.LinearAlgebra;
using Rationals;

namespace ReactionStoichiometry
{
    internal class RationalMatrix : CoreMatrix<Rational>
    {
        protected RationalMatrix(Matrix<double> matrix) : base(matrix, x => Rational.ParseDecimal(x.ToString()))
        {
            Basics = new Basics_<Rational>
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