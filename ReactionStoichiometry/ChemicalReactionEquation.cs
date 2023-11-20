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

        [JsonProperty(PropertyName = "Elements")]
        internal readonly IReadOnlyList<String> ChemicalElements;

        [JsonProperty(PropertyName = "Nullity")]
        internal readonly Int32 CompositionMatrixNullity;

        [JsonProperty(PropertyName = "Rank")]
        internal readonly Int32 CompositionMatrixRank;

        [JsonProperty(PropertyName = "OriginalEquationString")]
        internal readonly String InOriginalForm;

        [JsonProperty(PropertyName = "Labels")]
        internal readonly IReadOnlyList<String> Labels;

        // ReSharper disable once InconsistentNaming
        [JsonProperty(PropertyName = "RREF")] [JsonConverter(typeof(RationalArrayJsonConverter))]
        internal readonly Rational[,] RREF;

        [JsonIgnore]
        public String InGeneralForm =>
            StringOperations.AssembleEquationString(Substances
                                                  , Labels
                                                  , omitIf: static _ => false
                                                  , adapter: static s => s
                                                  , goesToRhsIf: static _ => false
                                                  , allowEmptyRhs: true);

        public ChemicalReactionEquation(String equationString)
        {
            InOriginalForm = equationString.Replace(oldValue: " ", String.Empty);

            AppSpecificException.ThrowIf(!IsValidString(InOriginalForm), message: "Invalid string");

            Substances = InOriginalForm.Split('=', '+').Where(predicate: static s => s != "0").ToList();
            Labels = Enumerable.Range(start: 0, Substances.Count).Select(selector: static i => 'x' + (i + 1).ToString(format: "D2")).ToList();
            FillCompositionMatrix(out CCM, out ChemicalElements);
            RREF = CCM.GetRREF(trim: true);
            CompositionMatrixRank = RREF.RowCount();
            CompositionMatrixNullity = CCM.ColumnCount() - CompositionMatrixRank;

            GeneralizedSolution = new SolutionGeneralized(this);
            InverseBasedSolution = new SolutionInverseBased(this);
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
            var elements = Regex.Matches(InOriginalForm, StringOperations.ELEMENT_SYMBOL)
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
