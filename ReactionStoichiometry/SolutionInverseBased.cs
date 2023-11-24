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

        [JsonProperty(PropertyName = "IndependentSetsOfCoefficients")]
        public readonly ReadOnlyCollection<BigInteger[]>? IndependentSetsOfCoefficients;

        [JsonProperty(PropertyName = "InverseMatrix")]
        [JsonConverter(typeof(RationalArrayJsonConverter))]
        internal readonly Rational[,]? InverseMatrix;

        internal SolutionInverseBased(ChemicalReactionEquation equation) : base(equation)
        {
            try
            {
                AppSpecificException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");
                AppSpecificException.ThrowIf(Equation.RREF.RowCount() >= Equation.RREF.ColumnCount(), message: "The method fails on this kind of equations");

                {
                    var square = new Rational[Equation.RREF.ColumnCount(), Equation.RREF.ColumnCount()];
                    Array.Copy(Equation.RREF, square, Equation.RREF.Length);
                    for (var r = Equation.RREF.RowCount(); r < square.RowCount(); r++)
                    {
                        for (var c = 0; c < square.ColumnCount(); c++)
                        {
                            square[r, c] = r == c ? 1 : 0;
                        }
                    }

                    InverseMatrix = square.GetInverse();
                }

                IndependentSetsOfCoefficients = Enumerable
                                                .Range(InverseMatrix.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                                .Select(selector: c => InverseMatrix.Column(c).ScaleToIntegers())
                                                .ToList()
                                                .AsReadOnly();

                CombinationSample = (null, null);
                if (IndependentSetsOfCoefficients.Count > 1)
                {
                    foreach (var recipe in Helpers.GeneratePermutations(IndependentSetsOfCoefficients.Count, maxValue: 5))
                    {
                        var combination = GetCombinationOfIndependents(recipe);
                        if (combination != null)
                        {
                            CombinationSample = (recipe, combination);
                            break;
                        }
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
                                             , String.Join(separator: ", ", IndependentSetsOfCoefficients!.Select(StringOperations.CoefficientsAsString)));
                AsMultilineString = String.Join(Environment.NewLine, IndependentSetsOfCoefficients!.Select(Equation.EquationWithIntegerCoefficients));
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
            var result = new BigInteger[IndependentSetsOfCoefficients![index: 0].Length];

            for (var r = 0; r < IndependentSetsOfCoefficients.Count; r++)
            {
                for (var c = 0; c < result.Length; c++)
                {
                    result[c] += IndependentSetsOfCoefficients[r][c] * weights[r];
                }
            }

            return result.Any(predicate: static v => v.IsZero) ? null : result.Select(selector: static i => new Rational(i)).ToArray().ScaleToIntegers();
        }
    }
}
