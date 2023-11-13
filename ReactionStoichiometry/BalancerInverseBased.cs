using System.Collections.ObjectModel;
using System.Numerics;
using Rationals;


namespace ReactionStoichiometry
{
    public sealed class BalancerInverseBased : Balancer
    {
        private List<BigInteger[]>? _independentReactions;

        internal ReadOnlyCollection<BigInteger[]> SolutionSets => _independentReactions?.AsReadOnly() ?? throw new InvalidOperationException();

        // ReSharper disable once MemberCanBeInternal
        public BalancerInverseBased(String equation) : base(equation)
        {
        }

        public override String ToString(OutputFormat format)
        {
            if (format != OutputFormat.SingleLine)
            {
                return base.ToString(format);
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_independentReactions == null)
            {
                return GlobalConstants.FAILURE_MARK;
            }

            return EquationWithPlaceholders()
                 + " with coefficients "
                 + String.Join(separator: ", ", _independentReactions.Select(selector: static i => i.ToCoefficientNotationString()));
        }

        protected override IEnumerable<String> Outcome()
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_independentReactions == null)
            {
                return new[] { GlobalConstants.FAILURE_MARK };
            }

            return _independentReactions.Select(EquationWithIntegerCoefficients);
        }

        protected override void Balance()
        {
            AppSpecificException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");

            Rational[,] inverse;
            {
                AppSpecificException.ThrowIf(Equation.RREF.RowCount() >= Equation.RREF.ColumnCount()
                                           , message: "The method fails on this kind of equations");

                var square = new Rational[Equation.RREF.ColumnCount(), Equation.RREF.ColumnCount()];
                Array.Copy(Equation.RREF, square, Equation.RREF.Length);
                for (var r = Equation.RREF.RowCount(); r < square.RowCount(); r++)
                {
                    for (var c = 0; c < square.ColumnCount(); c++)
                    {
                        square[r, c] = r == c ? 1 : 0;
                    }
                }

                inverse = RationalMatrixOperations.GetInverse(square);
            }

            _independentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                              .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                              .ToList();
        }
    }
}
