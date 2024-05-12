using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionColumnsBased : Solution
    {
        [JsonProperty(PropertyName = "CombinationSample")]
        public readonly (Int32[]? recipe, BigInteger[]? coefficients) CombinationSample;

        [JsonProperty(PropertyName = "IndependentSetsOfCoefficients")]
        public readonly ReadOnlyCollection<BigInteger[]>? IndependentSetsOfCoefficients;

        [JsonProperty(PropertyName = "InverseMatrix")]
        internal readonly Rational[,]? InverseMatrix;

        internal SolutionColumnsBased(ChemicalReactionEquation equation) : base(equation)
        {
            try
            {
                AppSpecificException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");
                AppSpecificException.ThrowIf(Equation.RREF.RowCount() >= Equation.RREF.ColumnCount()
                                           , message: "The method fails on equations like this"); //todo: provoke

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
                    var countOfReactantsInOriginal = equation.OriginalString.Split(separator: "=")[0].Split(separator: "+").Length;
                    foreach (var combination in Helpers.GeneratePermutations(IndependentSetsOfCoefficients.Count, maxValue: 10))
                    {
                        var candidate = CombineIndependents(combination);
                        if (candidate.Take(countOfReactantsInOriginal).Any(predicate: static i => i > 0))
                        {
                            continue;
                        }
                        if (candidate.Skip(countOfReactantsInOriginal).Any(predicate: static i => i < 0))
                        {
                            continue;
                        }
                        if (candidate.Any(predicate: static i => i == 0))
                        {
                            continue;
                        }

                        CombinationSample = (combination, candidate);
                        break;
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
        }

        public BigInteger[] CombineIndependents(params Int32[] combination)
        {
            return Equation.Substances
                           .Select(selector: (_, c) => IndependentSetsOfCoefficients!.Select(selector: (rowData, r) => rowData[c] * combination[r])
                                                                                     .Aggregate(BigInteger.Zero, func: static (acc, val) => acc + val))
                           .Select(selector: static i => new Rational(i))
                           .ToArray()
                           .ScaleToIntegers();
        }

        public Int32[]? FindCombination(params BigInteger[] coefficients)
        {
            // solving Ax=B
            // where A is a matrix, x and B are vectors
            // x is Inv(A)*B
            ArgumentNullException.ThrowIfNull(IndependentSetsOfCoefficients);

            var matrixA = new Rational[IndependentSetsOfCoefficients.Count, IndependentSetsOfCoefficients.Count];
            var vectorB = new BigInteger[IndependentSetsOfCoefficients.Count];

            Int32 i = 0, j = 0;
            while (i < IndependentSetsOfCoefficients.Count)
            {
                var candidateRowOfA = IndependentSetsOfCoefficients.Select(selector: v => v[i]).ToArray();
                if (candidateRowOfA.All(predicate: static v => v != 0))
                {
                    for (var k = 0; k < candidateRowOfA.Length; k++)
                    {
                        matrixA[j, k] = candidateRowOfA[k];
                    }
                    vectorB[j] = coefficients[i];
                    j++;
                }
                i++;
            }

            var vectorX = matrixA.GetInverse().MultiplyByVector(vectorB).ToArray();
            if (vectorX.Any(predicate: static r => r.Denominator != 1))
            {
                return null;
            }
            var candidate = vectorX.Select(selector: static r => (Int32)r.Numerator).ToArray();

            return CombineIndependents(candidate).SequenceEqual(coefficients) ? candidate : null;
        }
    }
}
