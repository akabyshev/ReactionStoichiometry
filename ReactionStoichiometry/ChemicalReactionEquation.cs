using System.Text.RegularExpressions;

using Rationals;

namespace ReactionStoichiometry
{
    public sealed class ChemicalReactionEquation
    {
        public readonly List<String> Substances;

        // ReSharper disable once InconsistentNaming
        internal readonly Rational[,] CCM;
        internal readonly Int32 CompositionMatrixRank;

        // ReSharper disable once InconsistentNaming
        internal readonly Rational[,] RREF;
        internal readonly String Skeletal;

        internal Int32 CompositionMatrixNullity => CCM.ColumnCount() - CompositionMatrixRank;

        internal ChemicalReactionEquation(String s)
        {
            if (!s.LooksLikeChemicalReactionEquation())
            {
                throw new ArgumentException(message: "Invalid string");
            }
            Skeletal = s;

            Substances = Skeletal.Split('=', '+').Where(predicate: static s => s != "0").ToList();
            CCM = GetCompositionMatrix();
            RREF = CCM.GetRREF(trim: true);
            CompositionMatrixRank = RREF.RowCount();
        }

        private Rational[,] GetCompositionMatrix()
        {
            var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
            var elements = Regex.Matches(Skeletal, StringOperations.ELEMENT_SYMBOL)
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
    }
}
