using System.Numerics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class ChemicalReactionEquation
    {
        [JsonProperty(PropertyName = "GeneralizedSolution")]
        public readonly SolutionGeneralized GeneralizedSolution;

        [JsonProperty(PropertyName = "InverseBasedSolution")]
        public readonly SolutionInverseBased InverseBasedSolution;

        [JsonProperty(PropertyName = "Substances")]
        public readonly IReadOnlyList<String> Substances;

        // ReSharper disable once InconsistentNaming
        [JsonProperty(PropertyName = "CCM")] [JsonConverter(typeof(RationalArrayJsonConverter))]
        internal readonly Rational[,] CCM;

        [JsonProperty(PropertyName = "Nullity")]
        internal readonly Int32 CompositionMatrixNullity;

        [JsonProperty(PropertyName = "Rank")]
        internal readonly Int32 CompositionMatrixRank;

        [JsonProperty(PropertyName = "Elements")]
        internal readonly IReadOnlyList<String> Elements;

        [JsonProperty(PropertyName = "Labels")]
        internal readonly IReadOnlyList<String> Labels;

        [JsonProperty(PropertyName = "OriginalInput")]
        internal readonly String OriginalEquation;

        // ReSharper disable once InconsistentNaming
        [JsonProperty(PropertyName = "RREF")] [JsonConverter(typeof(RationalArrayJsonConverter))]
        internal readonly Rational[,] RREF;

        internal readonly Int32[] SpecialColumnsIndices;

        [JsonIgnore]
        public String GeneralizedEquation =>
            StringOperations.AssembleEquationString(Substances
                                                  , Labels
                                                  , omitIf: static _ => false
                                                  , adapter: static s => s
                                                  , goesToRhsIf: static _ => false
                                                  , allowEmptyRhs: true);

        public ChemicalReactionEquation(String equationString)
        {
            OriginalEquation = equationString.Replace(oldValue: " ", String.Empty);

            if (!IsValidString(OriginalEquation))
            {
                throw new ArgumentException(message: "Invalid string");
            }

            Substances = OriginalEquation.Split('=', '+').Where(predicate: static s => s != "0").ToList();
            Labels = Enumerable.Range(start: 0, Substances.Count).Select(selector: static i => 'x' + (i + 1).ToString(format: "D2")).ToList();
            FillCompositionMatrix(out CCM, out Elements);
            RREF = CCM.GetRREF(trim: true);
            CompositionMatrixRank = RREF.RowCount();
            CompositionMatrixNullity = CCM.ColumnCount() - CompositionMatrixRank;

            SpecialColumnsIndices = Enumerable.Range(start: 0, RREF.ColumnCount()).Where(predicate: c => !ContainsOnlySingleOne(RREF.Column(c))).ToArray();

            GeneralizedSolution = new SolutionGeneralized(this);
            InverseBasedSolution = new SolutionInverseBased(this);

            return;

            static Boolean ContainsOnlySingleOne(Rational[] array)
            {
                return array.Count(predicate: static r => !r.IsZero) == 1 && array.Count(predicate: static r => r.IsOne) == 1;
            }
        }

        public String EquationWithIntegerCoefficients(BigInteger[] coefficients)
        {
            return StringOperations.AssembleEquationString(Substances
                                                         , coefficients
                                                         , omitIf: static value => value.IsZero
                                                         , adapter: static value => BigInteger.Abs(value) == 1 ? String.Empty : BigInteger.Abs(value).ToString()
                                                         , goesToRhsIf: static value => value > 0
                                                         , allowEmptyRhs: false);
        }

        public static Boolean IsValidString(String equationString)
        {
            return StringOperations.LooksLikeChemicalReactionEquation(equationString.Replace(oldValue: " ", String.Empty));
        }

        public BigInteger[] Instantiate(BigInteger[] freeVarsValues)
        {
            if (freeVarsValues.Length != SpecialColumnsIndices.Length)
            {
                throw new ArgumentException(message: "Array size mismatch", nameof(freeVarsValues));
            }

            var result = new BigInteger[Substances.Count];

            for (var i = 0; i < SpecialColumnsIndices.Length; i++)
            {
                result[SpecialColumnsIndices[i]] = freeVarsValues[i];
            }

            var rowIndex = -1;
            foreach (var i in Enumerable.Range(start: 0, result.Length).Except(SpecialColumnsIndices))
            {
                rowIndex++;

                var row = RREF.Row(rowIndex);
                row[Array.FindIndex(row, match: static r => r.IsOne)] = 0; // remove the leading 1
                var calculated = SpecialColumnsIndices.Aggregate(Rational.Zero, func: (cumulative, c) => cumulative + (row[c] * result[c]).CanonicalForm);
                AppSpecificException.ThrowIf(calculated.Denominator != 1, message: "Non-integer coefficient, try other SLE params");
                result[i] = -calculated.Numerator;
            }

            return result;
        }

        public String ToJson()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.SerializeObject(this, settings);
        }

        internal Boolean Validate(BigInteger[] coefficients)
        {
            if (coefficients.Length != Substances.Count)
            {
                throw new ArgumentException(message: "Array size mismatch");
            }

            for (var r = 0; r < CCM.RowCount(); r++)
            {
                var sum = Rational.Zero;
                for (var c = 0; c < CCM.ColumnCount(); c++)
                {
                    sum += CCM[r, c] * coefficients[c];
                }
                if (sum != Rational.Zero)
                {
                    return false;
                }
            }

            return true;
        }

        private void FillCompositionMatrix(out Rational[,] matrix, out IReadOnlyList<String> outElements)
        {
            var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
            var elements = Regex.Matches(OriginalEquation, StringOperations.ELEMENT_SYMBOL)
                                .Select(selector: static m => m.Value)
                                .Except(pseudoElementsOfCharge)
                                .ToList();
            elements.AddRange(pseudoElementsOfCharge); // it is important to have those as trailing rows of the matrix

            matrix = new Rational[elements.Count, Substances.Count];
            for (var r = 0; r < elements.Count; r++)
            {
                for (var c = 0; c < Substances.Count; c++)
                {
                    matrix[r, c] = 0;
                }
            }

            for (var r = 0; r < elements.Count; r++)
            {
                Regex regex = new(StringOperations.ELEMENT_TEMPLATE.Replace(oldValue: "X", elements[r]));

                for (var c = 0; c < Substances.Count; c++)
                {
                    var matches = regex.Matches(StringOperations.UnfoldSubstance(Substances[c]));

                    Rational sum = 0;
                    for (var i = 0; i < matches.Count; i++)
                    {
                        var match = matches[i];
                        sum += Rational.ParseDecimal(match.Groups[groupnum: 1].Value);
                    }
                    matrix[r, c] += sum;
                }
            }

            var (indexE, indexQn, indexQp) = (elements.IndexOf(item: "{e}"), elements.IndexOf(item: "Qn"), elements.IndexOf(item: "Qp"));
            for (var c = 0; c < Substances.Count; c++)
            {
                matrix[indexE, c] = -matrix[indexQn, c] + matrix[indexQp, c];
                matrix[indexQn, c] = 0;
                matrix[indexQp, c] = 0;
            }

            Helpers.TrimAndGetCanonicalForms(ref matrix);
            outElements = elements.Take(matrix.RowCount()).ToList().AsReadOnly();
        }
    }
}
