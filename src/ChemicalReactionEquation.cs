namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using Rationals;

public sealed class ChemicalReactionEquation
{
    // ReSharper disable once InconsistentNaming
    internal readonly Rational[,] CCM;
    internal readonly Rational[,] MagicMatrix;
    internal readonly Int32 CompositionMatrixRank;
    internal readonly Int32 OriginalReactantsCount;
    internal readonly String Skeletal;
    internal readonly List<String> Substances;

    internal Int32 CompositionMatrixNullity => CCM.GetLength(dimension: 1) - CompositionMatrixRank;

    internal ChemicalReactionEquation(String s)
    {
        if (!StringOperations.SeemsFine(s)) throw new ArgumentException(message: "Invalid string");
        Skeletal = s;
        OriginalReactantsCount = 1 + Skeletal.Split(separator: '=')[0].Count(predicate: static c => c == '+');

        Substances = Skeletal.Split('=', '+').ToList();
        CCM = GetCompositionMatrix();
        MagicMatrix = RationalArrayOperations.GetSpecialForm(CCM);
        CompositionMatrixRank = MagicMatrix.GetLength(dimension: 0);
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

        RationalArrayOperations.Normalize(ref result);
        return result;
    }
}
