using System.Numerics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public sealed class ChemicalReactionEquation
    {
        public readonly List<String> Substances;

        // ReSharper disable once InconsistentNaming
        [JsonProperty(PropertyName = "CCM")] [JsonConverter(typeof(RationalArrayJsonConverter))]
        internal readonly Rational[,] CCM;

        [JsonProperty(PropertyName = "CCM nullity")]
        internal readonly Int32 CompositionMatrixNullity;

        [JsonProperty(PropertyName = "CCM rank")]
        internal readonly Int32 CompositionMatrixRank;

        [JsonProperty(PropertyName = "Original input", Order = -2)]
        internal readonly String OriginalEquation;

        // ReSharper disable once InconsistentNaming
        [JsonProperty(PropertyName = "RREF")] [JsonConverter(typeof(RationalArrayJsonConverter))]
        internal readonly Rational[,] RREF;

        [JsonIgnore]
        public String GeneralizedEquation =>
            StringOperations.AssembleEquationString(Substances
                                                  , Enumerable.Range(start: 0, Substances.Count).Select(LabelFor).ToArray()
                                                  , omitIf: static _ => false
                                                  , adapter: static s => s
                                                  , goesToRhsIf: static _ => false
                                                  , allowEmptyRhs: true);

        internal ChemicalReactionEquation(String equationString)
        {
            OriginalEquation = equationString.Replace(oldValue: " ", String.Empty);

            if (!IsValidString(OriginalEquation))
            {
                throw new ArgumentException(message: "Invalid string");
            }

            Substances = OriginalEquation.Split('=', '+').Where(predicate: static s => s != "0").ToList();
            CCM = GetCompositionMatrix();
            RREF = CCM.GetRREF(trim: true);
            CompositionMatrixRank = RREF.RowCount();
            CompositionMatrixNullity = CCM.ColumnCount() - CompositionMatrixRank;
        }

        public String LabelFor(Int32 i)
        {
            return Substances.Count > GlobalConstants.LETTER_LABEL_THRESHOLD ? 'x' + (i + 1).ToString(format: "D2") : ((Char)('a' + i)).ToString();
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

        internal Boolean ValidateSolution(BigInteger[] coefficients)
        {
            if (coefficients.Length != Substances.Count)
            {
                throw new ArgumentException(message: "Size mismatch");
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

        private Rational[,] GetCompositionMatrix()
        {
            var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
            var elements = Regex.Matches(OriginalEquation, StringOperations.ELEMENT_SYMBOL)
                                .Select(selector: static m => m.Value)
                                .Except(pseudoElementsOfCharge)
                                .ToList();
            elements.AddRange(pseudoElementsOfCharge); // it is important to have those as trailing rows of the matrix

            var result = new Rational[elements.Count, Substances.Count];
            for (var r = 0; r < elements.Count; r++)
            {
                for (var c = 0; c < Substances.Count; c++)
                {
                    result[r, c] = 0;
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
                    result[r, c] += sum;
                }
            }

            var (indexE, indexQn, indexQp) = (elements.IndexOf(item: "{e}"), elements.IndexOf(item: "Qn"), elements.IndexOf(item: "Qp"));
            for (var c = 0; c < Substances.Count; c++)
            {
                result[indexE, c] = -result[indexQn, c] + result[indexQp, c];
                result[indexQn, c] = 0;
                result[indexQp, c] = 0;
            }

            Helpers.TrimAndGetCanonicalForms(ref result);
            return result;
        }

        public static Boolean IsValidString(String equationString) =>
            StringOperations.LooksLikeChemicalReactionEquation(equationString.Replace(oldValue: " ", String.Empty));
    }
}
