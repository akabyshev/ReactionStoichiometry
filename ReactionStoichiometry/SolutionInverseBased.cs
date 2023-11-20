using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionInverseBased : Solution
    {
        [JsonProperty(PropertyName = "CombinationSample")]
        internal readonly (Int32[]? weights, BigInteger[]? resultingCoefficients) CombinationSample;

        [JsonProperty(PropertyName = "IndependentReactions")]
        internal readonly ReadOnlyCollection<BigInteger[]>? IndependentReactions;

        internal SolutionInverseBased(ChemicalReactionEquation equation) : base(equation)
        {
            try
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

                    inverse = square.GetInverse();
                }

                IndependentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                                 .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                                 .ToList()
                                                 .AsReadOnly();
                if (IndependentReactions.Count == 1)
                {
                    CombinationSample = (null, null);
                }
                else
                {
                    var weights = new Int32[IndependentReactions.Count];

                    for (var i = 0; i < weights.Length; i++)
                    {
                        weights[i] = i + 1; //weights[weights.Length - 1 - i] = i + 1;
                    }

                    var combination = GetCombinationOfIndependents(weights);

                    if (combination != null)
                    {
                        CombinationSample = (weights, combination);
                    }
                }

                Success = true;
            }
            catch (AppSpecificException e)
            {
                FailureMessage = "This equation can't be balanced: " + e.Message;
                Success = false;
            }

            if (Success)
            {
                AsSimpleString = String.Format(format: "{0} with coefficients {1}"
                                             , Equation.InGeneralForm
                                             , String.Join(separator: ", ", IndependentReactions!.Select(StringOperations.CoefficientsAsString)));
                AsMultilineString = String.Join(Environment.NewLine, IndependentReactions!.Select(Equation.EquationWithIntegerCoefficients));
            }
            else
            {
                AsSimpleString = GlobalConstants.FAILURE_MARK;
                AsMultilineString = GlobalConstants.FAILURE_MARK;
            }
            AsDetailedMultilineString = GetAsDetailedMultilineString();
        }

        internal BigInteger[]? GetCombinationOfIndependents(IReadOnlyList<Int32> weights)
        {
            var result = new BigInteger[IndependentReactions![index: 0].Length];

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
