using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public sealed class SolutionGeneralized : Solution
    {
        [JsonProperty(PropertyName = "AlgebraicExpressions")]
        public readonly ReadOnlyCollection<String>? AlgebraicExpressions;

        [JsonProperty(PropertyName = "SimplestSolution")]
        public readonly ReadOnlyCollection<BigInteger>? GuessedSimplestSolution;

        [JsonProperty(PropertyName = "FreeVariableIndices")]
        internal readonly ReadOnlyCollection<Int32> FreeCoefficientIndices;

        private readonly Dictionary<Int32, BigInteger[]> _dependentCoefficientExpressions = new();

        internal SolutionGeneralized(ChemicalReactionEquation equation)
        {
            FreeCoefficientIndices = equation.SpecialColumnsIndices.AsReadOnly();

            try
            {
                AppSpecificException.ThrowIf(equation.RREF.IsIdentityMatrix(), message: "SLE is unsolvable");

                _dependentCoefficientExpressions = Enumerable.Range(start: 0, equation.RREF.RowCount())
                                                             .Select(selector: r => equation.RREF.Row(r)
                                                                                            .ScaleToIntegers()
                                                                                            .Select(selector: static i => -i)
                                                                                            .ToArray())
                                                             .ToDictionary(keySelector: static row => Array.FindIndex(row, match: static i => i != 0)
                                                                         , elementSelector: static row => row);

                GuessedSimplestSolution = FreeCoefficientIndices.Count != 1 ?
                    null :
                    equation.Instantiate(new[]
                                         {
                                             equation.RREF.Column(FreeCoefficientIndices[index: 0])
                                                     .Select(selector: static r => r.Denominator)
                                                     .Aggregate(Helpers.LeastCommonMultiple)
                                         })
                            .AsReadOnly();

                AlgebraicExpressions = Enumerable.Range(start: 0, equation.Substances.Count)
                                                 .Select(selector: i => FreeCoefficientIndices.Contains(i) ?
                                                                       equation.Labels[i] :
                                                                       String.Format(format: "{0} = {1}"
                                                                                   , equation.Labels[i]
                                                                                   , AlgebraicExpressionForCoefficient(i)))
                                                 .ToList()
                                                 .AsReadOnly();
                Success = true;
            }
            catch (AppSpecificException e)
            {
                FailureMessage = "This equation can't be balanced: " + e.Message;
                Success = false;
            }


            AsSimpleString = AlgebraicExpressions == null ?
                GlobalConstants.FAILURE_MARK :
                String.Format(format: "{0} with coefficients {1}"
                            , equation.GeneralizedEquation
                            , AlgebraicExpressions.Select(selector: static s => !s.Contains(value: " = ") ? s : s.Split(separator: " = ")[1])
                                                  .CoefficientsAsString());
            AsMultilineString = AlgebraicExpressions == null ?
                GlobalConstants.FAILURE_MARK :
                String.Format(format: "{0} with coefficients{3}{1}{3}for any {2}"
                            , equation.GeneralizedEquation
                            , String.Join(Environment.NewLine, AlgebraicExpressions.Where(predicate: static s => s.Contains(value: " = ")))
                            , FreeCoefficientIndices.Select(selector: i => equation.Labels[i]).CoefficientsAsString()
                            , Environment.NewLine);
            AsDetailedMultilineString = GetAsDetailedMultilineString(equation);

            return;

            String AlgebraicExpressionForCoefficient(Int32 index)
            {
                if (!_dependentCoefficientExpressions.TryGetValue(index, out var expression))
                {
                    return null!;
                }

                var numeratorParts = Enumerable.Range(index + 1, expression.Length - (index + 1))
                                               .Where(predicate: i => expression[i] != 0)
                                               .Select(selector: i =>
                                                                 {
                                                                     var coefficient = expression[i] + GlobalConstants.MULTIPLICATION_SYMBOL;
                                                                     if (expression[i] == 1)
                                                                     {
                                                                         coefficient = String.Empty;
                                                                     }
                                                                     if (expression[i] == -1)
                                                                     {
                                                                         coefficient = "-";
                                                                     }
                                                                     return $"{coefficient}{equation.Labels[i]}";
                                                                 })
                                               .ToList();

                var result = String.Join(separator: " + ", numeratorParts).Replace(oldValue: "+ -", newValue: "- ");

                if (expression[index] != -1)
                {
                    if (numeratorParts.Count > 1)
                    {
                        result = $"({result})";
                    }
                    result = $"{result}/{BigInteger.Abs(expression[index])}";
                }

                if (result == String.Empty)
                {
                    result = "0";
                }

                return result;
            }
        }
    }
}
