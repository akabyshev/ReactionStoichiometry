using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionColumnsBased : Solution
    {
        [JsonProperty(PropertyName = "IndependentSetsOfCoefficients")]
        public readonly ReadOnlyCollection<BigInteger[]>? IndependentSetsOfCoefficients;

        [JsonProperty(PropertyName = "CombinationSample")]
        public readonly (Int32[]? recipe, BigInteger[]? coefficients) CombinationSample;

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
                    foreach (var combination in Helpers.GeneratePermutations(IndependentSetsOfCoefficients.Count, maxValue: 10))
                    {
                        var candidate = CombineIndependents(combination);
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
                           .Select((_, c) => IndependentSetsOfCoefficients!.Select((rowData, r) => rowData[c] * combination[r])
                                                                           .Aggregate(BigInteger.Zero, static (acc, val) => acc + val))
                           .Select(selector: static i => new Rational(i))
                           .ToArray()
                           .ScaleToIntegers();
        }

        public Int32[] FindCombination(BigInteger[] coefficients)
        {
            return Enumerable.Repeat(element: 1, coefficients.Length).ToArray(); //todo
        }
    }
}
