using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class BalancerInverseBased : Balancer
    {
        [JsonProperty(PropertyName = "Independent reactions")]
        private List<BigInteger[]>? _independentReactions;

        internal ReadOnlyCollection<BigInteger[]> SolutionSets => _independentReactions?.AsReadOnly() ?? throw new InvalidOperationException();

        public BalancerInverseBased(String equation) : base(equation)
        {
        }

        public override String ToString(OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Simple or OutputFormat.Multiline when _independentReactions == null => GlobalConstants.FAILURE_MARK
              , OutputFormat.Simple => String.Format(format: "{0} with coefficients {1}"
                                                   , Equation.GeneralizedEquation
                                                   , String.Join(separator: ", ", _independentReactions.Select(StringOperations.CoefficientsAsString)))
              , OutputFormat.Multiline => String.Join(Environment.NewLine, _independentReactions.Select(Equation.EquationWithIntegerCoefficients))
              , _ => base.ToString(format)
            };
        }

        protected override void BalanceImplementation()
        {
            AppSpecificException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");

            Rational[,] inverse;
            {
                AppSpecificException.ThrowIf(Equation.RREF.RowCount() >= Equation.RREF.ColumnCount(), message: "The method fails on this kind of equations");

                var square = new Rational[Equation.RREF.ColumnCount(), Equation.RREF.ColumnCount()];
                Array.Copy(Equation.RREF, square, Equation.RREF.Length);
                for (var r = Equation.RREF.RowCount(); r < square.RowCount(); r++)
                {
                    for (var c = 0; c < square.ColumnCount(); c++)
                    {
                        square[r, c] = r == c ? 1 : 0;
                    }
                }

                inverse = square.GetInverse();
            }

            _independentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                              .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                              .ToList();
        }
    }
}
