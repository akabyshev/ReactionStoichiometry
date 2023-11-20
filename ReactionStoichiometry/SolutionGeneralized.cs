using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionGeneralized : Solution
    {
        [JsonProperty(PropertyName = "AlgebraicExpressions")]
        public readonly ReadOnlyCollection<String>? AlgebraicExpressions;

        [JsonProperty(PropertyName = "SimplestSolution")]
        public readonly ReadOnlyCollection<BigInteger>? SimplestSolution;
        // this can not be a tuple unlike CombinationSample of SolutionInverseBased, because the parameter is contained already

        [JsonProperty(PropertyName = "FreeVariableIndices")]
        internal readonly ReadOnlyCollection<Int32>? FreeCoefficientIndices;

        private readonly Dictionary<Int32, BigInteger[]> _dependentCoefficientExpressions = new();

        internal SolutionGeneralized(ChemicalReactionEquation equation) : base(equation)
        {
            try
            {
                AppSpecificException.ThrowIf(Equation.RREF.IsIdentityMatrix(), message: "SLE is unsolvable");

                FreeCoefficientIndices = Enumerable.Range(start: 0, Equation.RREF.ColumnCount())
                                                   .Where(predicate: c => !ContainsOnlySingleOne(Equation.RREF.Column(c)))
                                                   .ToList()
                                                   .AsReadOnly();

                _dependentCoefficientExpressions = Enumerable.Range(start: 0, Equation.RREF.RowCount())
                                                             .Select(selector: r => Equation.RREF.Row(r)
                                                                                            .ScaleToIntegers()
                                                                                            .Select(selector: static i => -i)
                                                                                            .ToArray())
                                                             .ToDictionary(keySelector: static row => Array.FindIndex(row, match: static i => i != 0)
                                                                         , elementSelector: static row => row);

                SimplestSolution = FreeCoefficientIndices.Count != 1 ?
                    null :
                    Instantiate(new[]
                                {
                                    Equation.RREF.Column(FreeCoefficientIndices[index: 0])
                                            .Select(selector: static r => r.Denominator)
                                            .Aggregate(Helpers.LeastCommonMultiple)
                                })
                        .AsReadOnly();

                AlgebraicExpressions = Enumerable.Range(start: 0, Equation.Substances.Count)
                                                 .Select(selector: i => FreeCoefficientIndices.Contains(i) ?
                                                                       Equation.Labels[i] :
                                                                       String.Format(format: "{0} = {1}"
                                                                                   , Equation.Labels[i]
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

            if (Success)
            {
                AsSimpleString = String.Format(format: "{0} with coefficients {1}"
                                             , Equation.InGeneralForm
                                             , AlgebraicExpressions!.Select(selector: static s => !s.Contains(value: " = ") ? s : s.Split(separator: " = ")[1])
                                                                    .CoefficientsAsString());
                AsMultilineString = String.Format(format: "{0} with coefficients{3}{1}{3}for any {2}"
                                                , Equation.InGeneralForm
                                                , String.Join(Environment.NewLine, AlgebraicExpressions!.Where(predicate: static s => s.Contains(value: " = ")))
                                                , FreeCoefficientIndices!.Select(selector: i => Equation.Labels[i]).CoefficientsAsString()
                                                , Environment.NewLine);
            }
            else
            {
                AsSimpleString = GlobalConstants.FAILURE_MARK;
                AsMultilineString = GlobalConstants.FAILURE_MARK;
            }
            AsDetailedMultilineString = GetAsDetailedMultilineString();

            return;

            static Boolean ContainsOnlySingleOne(Rational[] array)
            {
                return array.Count(predicate: static r => !r.IsZero) == 1 && array.Count(predicate: static r => r.IsOne) == 1;
            }

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
                                                                     return $"{coefficient}{Equation.Labels[i]}";
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

        public BigInteger[] Instantiate(BigInteger[] freeCoefficientValues)
        {
            AppSpecificException.ThrowIf(FreeCoefficientIndices == null, message: "Invalid call");

            if (freeCoefficientValues.Length != FreeCoefficientIndices!.Count)
            {
                throw new ArgumentException(message: "Array size mismatch", nameof(freeCoefficientValues));
            }

            var result = new BigInteger[Equation.Substances.Count];

            for (var i = 0; i < FreeCoefficientIndices.Count; i++)
            {
                result[FreeCoefficientIndices[i]] = freeCoefficientValues[i];
            }

            var rowIndex = -1;
            foreach (var i in Enumerable.Range(start: 0, result.Length).Except(FreeCoefficientIndices))
            {
                rowIndex++;

                var row = Equation.RREF.Row(rowIndex);
                row[Array.FindIndex(row, match: static r => r.IsOne)] = 0; // remove the leading 1
                var calculated = FreeCoefficientIndices.Aggregate(Rational.Zero, func: (cumulative, c) => cumulative + (row[c] * result[c]).CanonicalForm);
                AppSpecificException.ThrowIf(calculated.Denominator != 1, message: "Non-integer coefficient, try other SLE params");
                result[i] = -calculated.Numerator;
            }

            return result;
        }
    }
}
