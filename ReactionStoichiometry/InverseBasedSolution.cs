using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    internal sealed class InverseBasedSolution : Solution
    {
        internal override String Name => nameof(InverseBasedSolution);

        [JsonProperty(PropertyName = "Independent reactions")]
        private List<BigInteger[]>? _independentReactions;

        protected override void WorkOn_Impl(ChemicalReactionEquation equation)
        {
            AppSpecificException.ThrowIf(equation.CompositionMatrixNullity == 0, message: "Zero null-space");

            Rational[,] inverse;
            {
                AppSpecificException.ThrowIf(equation.RREF.RowCount() >= equation.RREF.ColumnCount(), message: "The method fails on this kind of equations");

                var square = new Rational[equation.RREF.ColumnCount(), equation.RREF.ColumnCount()];
                Array.Copy(equation.RREF, square, equation.RREF.Length);
                for (var r = equation.RREF.RowCount(); r < square.RowCount(); r++)
                {
                    for (var c = 0; c < square.ColumnCount(); c++)
                    {
                        square[r, c] = r == c ? 1 : 0;
                    }
                }

                inverse = square.GetInverse();
            }

            _independentReactions = Enumerable.Range(inverse.ColumnCount() - equation.CompositionMatrixNullity, equation.CompositionMatrixNullity)
                                              .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                              .ToList();

            AsSimpleString = String.Format(format: "{0} with coefficients {1}"
                                         , equation.GeneralizedEquation
                                         , String.Join(separator: ", ", _independentReactions.Select(StringOperations.CoefficientsAsString)));
            AsMultilineString = String.Join(Environment.NewLine, _independentReactions.Select(equation.EquationWithIntegerCoefficients));
        }

        public override String ToString(OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Simple or OutputFormat.Multiline when _independentReactions == null => GlobalConstants.FAILURE_MARK
              , OutputFormat.Simple => AsSimpleString
              , OutputFormat.Multiline => AsMultilineString
              , OutputFormat.DetailedMultiline => AsDetailedMultilineString
              , _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }
    }
}