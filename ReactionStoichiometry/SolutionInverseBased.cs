using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionInverseBased : Solution
    {
        [JsonProperty(PropertyName = "CombinationSample")]
        public readonly (String? weights, BigInteger[]? resultingCoefficients) CombinationSample = (null, null);

        [JsonProperty(PropertyName = "IndependentReactions")]
        internal readonly List<BigInteger[]> IndependentReactions = new();

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

                IndependentReactions = Enumerable.Range(inverse.ColumnCount() - equation.CompositionMatrixNullity, equation.CompositionMatrixNullity)
                                                 .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                                 .ToList();
                if (IndependentReactions.Count > 1)
                {
                    var weights = new Int32[IndependentReactions.Count];

                    for (var i = 0; i < weights.Length; i++)
                    {
                        weights[i] = i;
                    }

                    CombinationSample = (weights.CoefficientsAsString(), GetCombinationOfIndependents(weights));
                }

                Success = true;
            }
            catch (AppSpecificException e)
            {
                FailureMessage = "This equation can't be balanced: " + e.Message;
                Success = false;
            }


            AsSimpleString = IndependentReactions.Count == 0 ?
                GlobalConstants.FAILURE_MARK :
                String.Format(format: "{0} with coefficients {1}"
                            , equation.GeneralizedEquation
                            , String.Join(separator: ", ", IndependentReactions.Select(StringOperations.CoefficientsAsString)));
            AsMultilineString = IndependentReactions.Count == 0 ?
                GlobalConstants.FAILURE_MARK :
                String.Join(Environment.NewLine, IndependentReactions.Select(equation.EquationWithIntegerCoefficients));
            AsDetailedMultilineString = GetAsDetailedMultilineString(equation);
        }

        internal BigInteger[]? GetCombinationOfIndependents(IReadOnlyList<Int32> weights)
        {
            var result = new BigInteger[IndependentReactions[index: 0].Length];

            for (var r = 0; r < IndependentReactions.Count; r++)
            {
                for (var c = 0; c < result.Length; c++)
                {
                    result[c] += IndependentReactions[r][c] * weights[r];
                }
            }

            return result.Any(predicate: static v => v.IsZero) ? null : result.Select(selector: static i => new Rational(i)).ToArray().ScaleToIntegers();
        }
    }
}
