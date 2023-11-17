using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public sealed class BalancerGeneralized : Balancer
    {
        private Dictionary<Int32, BigInteger[]>? _dependentCoefficientExpressions;

        [JsonProperty(PropertyName = "Free variables")]
        private ReadOnlyCollection<Int32>? _freeCoefficientIndices;

        [JsonProperty(PropertyName = "Simplest solution")]
        public (BigInteger? singleFreeVarValue, String? balancedEquation) GuessedSimplestSolution
        {
            get
            {
                if (_freeCoefficientIndices is not { Count: 1 })
                {
                    return (null, null);
                }
                var value = Equation.RREF.Column(_freeCoefficientIndices[index: 0])
                                    .Select(selector: static r => r.Denominator)
                                    .Aggregate(Helpers.LeastCommonMultiple);
                return (value, Equation.EquationWithIntegerCoefficients(Equation.Instantiate(new[] { value })));
            }
        }

        [JsonProperty(PropertyName = "Algebraic expressions")]
        private String[]? AllAlgebraicExpressions =>
            _dependentCoefficientExpressions?.Keys.Select(
                                                selector: i => String.Format(format: "{0} = {1}", Equation.LabelFor(i), AlgebraicExpressionForCoefficient(i)))
                                            .ToArray();

        public BalancerGeneralized(String equationString) : this(new ChemicalReactionEquation(equationString))
        {
        }

        public BalancerGeneralized(ChemicalReactionEquation equation) : base(equation)
        {
        }

        public override String ToString(OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Simple or OutputFormat.Multiline when _dependentCoefficientExpressions == null || _freeCoefficientIndices == null =>
                    GlobalConstants.FAILURE_MARK
              , OutputFormat.Simple => String.Format(format: "{0} with coefficients {1}"
                                                   , Equation.GeneralizedEquation
                                                   , Enumerable.Range(start: 0, Equation.Substances.Count)
                                                               .Select(selector: i => _freeCoefficientIndices.Contains(i) ?
                                                                                     Equation.LabelFor(i) :
                                                                                     AlgebraicExpressionForCoefficient(i))
                                                               .CoefficientsAsString())
              , OutputFormat.Multiline => String.Format(format: "{0} with coefficients{3}{1}{3}for any {2}"
                                                      , Equation.GeneralizedEquation
                                                      , String.Join(Environment.NewLine, AllAlgebraicExpressions!)
                                                      , _freeCoefficientIndices.Select(Equation.LabelFor).CoefficientsAsString()
                                                      , Environment.NewLine)

              , _ => base.ToString(format)
            };
        }

        protected override void BalanceImplementation()
        {
            AppSpecificException.ThrowIf(Equation.RREF.IsIdentityMatrix(), message: "SLE is unsolvable");

            _dependentCoefficientExpressions = Enumerable.Range(start: 0, Equation.RREF.RowCount())
                                                         .Select(selector: r => Equation.RREF.Row(r)
                                                                                        .ScaleToIntegers()
                                                                                        .Select(selector: static i => -i)
                                                                                        .ToArray())
                                                         .ToDictionary(keySelector: static row => Array.FindIndex(row, match: static i => i != 0)
                                                                     , elementSelector: static row => row);

            _freeCoefficientIndices = Equation.SpecialColumnsIndices.AsReadOnly();
        }

        internal BigInteger[] Instantiate(BigInteger[] parameters) => Equation.Instantiate(parameters);

        public String AlgebraicExpressionForCoefficient(Int32 index)
        {
            if (_dependentCoefficientExpressions == null)
            {
                throw new InvalidOperationException();
            }

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
                                                                 return $"{coefficient}{Equation.LabelFor(i)}";
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
