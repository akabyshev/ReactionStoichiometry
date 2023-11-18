using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public sealed class GeneralizedSolution : Solution
    {
        private readonly List<String> _algebraicExpressions = new();
        private readonly Dictionary<Int32, BigInteger[]> _dependentCoefficientExpressions = new();

        [JsonProperty(PropertyName = "Free variables")]
        private readonly ReadOnlyCollection<Int32> _freeCoefficientIndices;

        [JsonProperty(PropertyName = "Algebraic expressions")]
        public ReadOnlyCollection<String> AlgebraicExpressions => _algebraicExpressions.AsReadOnly();

        [JsonProperty(PropertyName = "Simplest solution")]
        public BigInteger? GuessedSimplestSolution { get; private set; }

        internal override String Name => nameof(GeneralizedSolution);

        internal GeneralizedSolution(ChemicalReactionEquation equation)
        {
            _freeCoefficientIndices = equation.SpecialColumnsIndices.AsReadOnly();

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

                GuessedSimplestSolution = _freeCoefficientIndices.Count == 1 ?
                    equation.RREF.Column(_freeCoefficientIndices[index: 0]).Select(selector: static r => r.Denominator).Aggregate(Helpers.LeastCommonMultiple) :
                    null;

                _algebraicExpressions.AddRange(Enumerable.Range(start: 0, equation.Substances.Count)
                                                         .Select(selector: i => _freeCoefficientIndices.Contains(i) ?
                                                                               equation.LabelFor(i) :
                                                                               String.Format(format: "{0} = {1}"
                                                                                           , equation.LabelFor(i)
                                                                                           , AlgebraicExpressionForCoefficient(i))));
                Success = true;
            }
            catch (AppSpecificException e)
            {
                FailureMessage = "This equation can't be balanced: " + e.Message;
                Success = false;
            }


            AsSimpleString = _algebraicExpressions.Count == 0 ?
                GlobalConstants.FAILURE_MARK :
                String.Format(format: "{0} with coefficients {1}"
                            , equation.GeneralizedEquation
                            , _algebraicExpressions.Select(selector: static s => !s.Contains(value: " = ") ? s : s.Split(separator: " = ")[1])
                                                   .CoefficientsAsString());
            AsMultilineString = _algebraicExpressions.Count == 0 ?
                GlobalConstants.FAILURE_MARK :
                String.Format(format: "{0} with coefficients{3}{1}{3}for any {2}"
                            , equation.GeneralizedEquation
                            , String.Join(Environment.NewLine, _algebraicExpressions.Where(predicate: static s => s.Contains(value: " = ")))
                            , _freeCoefficientIndices.Select(equation.LabelFor).CoefficientsAsString()
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
                                                                     return $"{coefficient}{equation.LabelFor(i)}";
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
