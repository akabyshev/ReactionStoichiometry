using System.Numerics;

namespace ReactionStoichiometry
{
    public sealed class BalancerGeneralized : Balancer
    {
        private Dictionary<Int32, BigInteger[]>? _dependentCoefficientExpressions;
        private List<Int32>? _freeCoefficientIndices;

        public BalancerGeneralized(String equation) : base(equation)
        {
        }

        public override String ToString(OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Simple or OutputFormat.SeparateLines when _dependentCoefficientExpressions == null || _freeCoefficientIndices == null =>
                    GlobalConstants.FAILURE_MARK
              , OutputFormat.Simple => String.Format(format: "{0} with coefficients {1}"
                                                   , EquationWithPlaceholders()
                                                   , Enumerable.Range(start: 0, Equation.Substances.Count)
                                                               .Select(selector: i => _freeCoefficientIndices.Contains(i) ?
                                                                                     LabelFor(i) :
                                                                                     AlgebraicExpressionForCoefficient(i))
                                                               .CoefficientsAsString())
              , OutputFormat.SeparateLines => String.Format(format: "{0}{1}for any {2}"
                                                          , String.Join(Environment.NewLine
                                                                      , _dependentCoefficientExpressions.Keys.Select(
                                                                            selector: i => $"{LabelFor(i)} = {AlgebraicExpressionForCoefficient(i)}"))
                                                          , Environment.NewLine
                                                          , _freeCoefficientIndices.Select(LabelFor).CoefficientsAsString())
              , _ => base.ToString(format)
            };
        }

        protected override void Balance()
        {
            AppSpecificException.ThrowIf(Equation.RREF.IsIdentityMatrix(), message: "SLE is unsolvable");

            _dependentCoefficientExpressions = Enumerable.Range(start: 0, Equation.RREF.RowCount())
                                                         .Select(selector: r => Equation.RREF.Row(r)
                                                                                        .ScaleToIntegers()
                                                                                        .Select(selector: static i => -i)
                                                                                        .ToArray())
                                                         .ToDictionary(keySelector: static row => Array.FindIndex(row, match: static i => i != 0)
                                                                     , elementSelector: static row => row);

            _freeCoefficientIndices = Enumerable.Range(start: 0, Equation.RREF.ColumnCount())
                                                .Where(predicate: c => !_dependentCoefficientExpressions.ContainsKey(c)
                                                                    && Equation.RREF.Column(c).Any(predicate: static t => t != 0))
                                                .ToList();
        }

        public BigInteger[] Instantiate(BigInteger[] parameters)
        {
            if (parameters.Length != _freeCoefficientIndices!.Count)
            {
                throw new ArgumentException(message: "Array size mismatch", nameof(parameters));
            }

            var result = new BigInteger[Equation.Substances.Count];

            for (var i = 0; i < _freeCoefficientIndices.Count; i++)
            {
                result[_freeCoefficientIndices[i]] = parameters[i];
            }

            foreach (var kvp in _dependentCoefficientExpressions!)
            {
                var calculated = _freeCoefficientIndices.Aggregate(BigInteger.Zero, func: (sum, i) => sum + result[i] * kvp.Value[i]);
                AppSpecificException.ThrowIf(calculated % kvp.Value[kvp.Key] != 0, message: "Non-integer coefficient, try other SLE params");
                calculated /= kvp.Value[kvp.Key];

                result[kvp.Key] = -calculated;
            }

            return result;
        }

        public String AlgebraicExpressionForCoefficient(Int32 index)
        {
            if (_dependentCoefficientExpressions == null)
            {
                throw new InvalidOperationException();
            }

            if (!_dependentCoefficientExpressions.ContainsKey(index))
            {
                return null!;
            }

            var expression = _dependentCoefficientExpressions[index];

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
                                                                 return $"{coefficient}{LabelFor(i)}";
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
