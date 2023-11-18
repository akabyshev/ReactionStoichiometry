using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionInverseBased : Solution
    {
        [JsonProperty(PropertyName = "Independent reactions")]
        private readonly List<BigInteger[]> _independentReactions = new();

        internal SolutionInverseBased(ChemicalReactionEquation equation)
        {
            try
            {
                AppSpecificException.ThrowIf(equation.CompositionMatrixNullity == 0, message: "Zero null-space");

                Rational[,] inverse;
                {
                    AppSpecificException.ThrowIf(equation.RREF.RowCount() >= equation.RREF.ColumnCount()
                                               , message: "The method fails on this kind of equations");

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
                Success = true;
            }
            catch (AppSpecificException e)
            {
                FailureMessage = "This equation can't be balanced: " + e.Message;
                Success = false;
            }


            AsSimpleString = _independentReactions.Count == 0 ?
                GlobalConstants.FAILURE_MARK :
                String.Format(format: "{0} with coefficients {1}"
                            , equation.GeneralizedEquation
                            , String.Join(separator: ", ", _independentReactions.Select(StringOperations.CoefficientsAsString)));
            AsMultilineString = _independentReactions.Count == 0 ?
                GlobalConstants.FAILURE_MARK :
                String.Join(Environment.NewLine, _independentReactions.Select(equation.EquationWithIntegerCoefficients));
            AsDetailedMultilineString = GetAsDetailedMultilineString(equation);
        }
    }
}
