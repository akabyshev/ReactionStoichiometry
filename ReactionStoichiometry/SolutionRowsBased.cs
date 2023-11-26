using System.Collections.ObjectModel;
using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class SolutionRowsBased : Solution
    {
        [JsonProperty(PropertyName = "AlgebraicExpressions")]
        public readonly ReadOnlyCollection<String>? AlgebraicExpressions;

        [JsonProperty(PropertyName = "SimplestSolution")]
        public readonly ReadOnlyCollection<BigInteger>? SimplestSolution;
        // this can not be a tuple unlike CombinationSample of SolutionColumnsBased, because the parameter is contained already

        [JsonProperty(PropertyName = "FreeVariableIndices")]
        internal readonly ReadOnlyCollection<Int32>? FreeCoefficientIndices;

        internal SolutionRowsBased(ChemicalReactionEquation equation) : base(equation)
        {
            try
            {
                AppSpecificException.ThrowIf(Equation.RREF.IsIdentityMatrix(), message: "SLE is unsolvable");

                FreeCoefficientIndices = Equation.SpecialColumnsOfRREF;

                SimplestSolution = FreeCoefficientIndices.Count != 1 ?
                    null :
                    Instantiate(new[]
                                {
                                    Equation.RREF.Column(FreeCoefficientIndices[index: 0])
                                            .Select(selector: static r => r.Denominator)
                                            .Aggregate(Helpers.LeastCommonMultiple)
                                })
                        .AsReadOnly();

                AlgebraicExpressions = GetAllExpressions().AsReadOnly();

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

            List<String> GetAllExpressions()
            {
                var result = new List<String>();

                var dependentCoefficientExpressions = Enumerable.Range(start: 0, Equation.RREF.RowCount())
                                                                .Select(selector: r => Equation.RREF.Row(r)
                                                                                               .ScaleToIntegers()
                                                                                               .Select(selector: static i => -i)
                                                                                               .ToArray())
                                                                .ToDictionary(keySelector: static row => Array.FindIndex(row, match: static i => i != 0)
                                                                            , elementSelector: static row => row);

                for (var index = 0; index < Equation.Substances.Count; index++)
                {
                    if (FreeCoefficientIndices.Contains(index))
                    {
                        result.Add(Equation.Labels[index]);
                    }
                    else
                    {
                        var array = dependentCoefficientExpressions[index];

                        var numeratorParts = Enumerable.Range(index + 1, array.Length - (index + 1))
                                                       .Where(predicate: i => array[i] != 0)
                                                       .Select(selector: i =>
                                                                         {
                                                                             var coefficient = array[i] + GlobalConstants.MULTIPLICATION_SYMBOL;
                                                                             if (array[i] == 1)
                                                                             {
                                                                                 coefficient = String.Empty;
                                                                             }
                                                                             if (array[i] == -1)
                                                                             {
                                                                                 coefficient = "-";
                                                                             }
                                                                             return $"{coefficient}{Equation.Labels[i]}";
                                                                         })
                                                       .ToList();

                        var expression = String.Join(separator: " + ", numeratorParts).Replace(oldValue: "+ -", newValue: "- ");

                        if (array[index] != -1)
                        {
                            if (numeratorParts.Count > 1)
                            {
                                expression = $"({expression})";
                            }
                            expression = $"{expression}/{BigInteger.Abs(array[index])}";
                        }

                        if (expression == String.Empty)
                        {
                            expression = "0";
                        }

                        result.Add($"{Equation.Labels[index]} = {expression}");
                    }
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
                var calculated = FreeCoefficientIndices.Aggregate(Rational.Zero, func: (cumulative, c) => cumulative + row[c] * result[c]).CanonicalForm;
                AppSpecificException.ThrowIf(calculated.Denominator != 1, message: "Non-integer coefficient, try other SLE params");
                result[i] = -calculated.Numerator;
            }

            return result.Select(selector: static i => new Rational(i)).ToArray().ScaleToIntegers(); // return 1,2,3 if result is 5,10,15
        }
    }
}
