namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using Rationals;

public sealed class ChemicalReactionEquation
{
    private readonly List<String> _substances;
    internal readonly RationalMatrix CompositionMatrix;
    internal readonly Int32 OriginalReactantsCount;

    internal readonly String Skeletal;

    internal ChemicalReactionEquation(String s)
    {
        if (!StringOperations.SeemsFine(s)) throw new ArgumentException("Invalid string");
        Skeletal = s;
        OriginalReactantsCount = 1 + Skeletal.Split('=')[0].Count(static c => c == '+');

        _substances = Skeletal.Split('=', '+').ToList();
        CompositionMatrix = RationalMatrix.CreateInstance(GetCompositionMatrix(), static v => v);
    }

    public Int32 SubstancesCount => _substances.Count;
    public String GetSubstance(Int32 i) => _substances[i];

    internal IEnumerable<String> MatrixAsStrings()
    {
        var result = new List<String>();
        result.AddRange(CompositionMatrix.PrettyPrint("Chemical composition matrix", GetSubstance));
        result.Add(
            $"RxC: {CompositionMatrix.RowCount}x{CompositionMatrix.ColumnCount}, rank = {CompositionMatrix.Rank}, nullity = {CompositionMatrix.Nullity}");

        return result;
    }

    private Rational[,] GetCompositionMatrix()
    {
        var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
        var elements = Regex.Matches(Skeletal, StringOperations.ELEMENT_SYMBOL).Select(static m => m.Value).Except(pseudoElementsOfCharge).ToList();
        elements.AddRange(pseudoElementsOfCharge); // it is important to have those as trailing rows of the matrix

        var result = new Rational[elements.Count, _substances.Count];
        for (var r = 0; r < elements.Count; r++)
        {
            for (var c = 0; c < _substances.Count; c++)
            {
                result[r, c] = 0;
            }
        }

        for (var r = 0; r < elements.Count; r++)
        {
            Regex regex = new(StringOperations.ELEMENT_TEMPLATE.Replace("X", elements[r]));

            for (var c = 0; c < _substances.Count; c++)
            {
                var matches = regex.Matches(StringOperations.UnfoldSubstance(_substances[c]));

                Rational sum = 0;
                for (var i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    sum += Rational.ParseDecimal(match.Groups[1].Value);
                }
                result[r, c] += sum;
            }
        }

        var (indexE, indexQn, indexQp) = (elements.IndexOf("{e}"), elements.IndexOf("Qn"), elements.IndexOf("Qp"));
        for (var c = 0; c < _substances.Count; c++)
        {
            result[indexE, c] = -result[indexQn, c] + result[indexQp, c];
            result[indexQn, c] = 0;
            result[indexQp, c] = 0;
        }

        return Utils.WithoutTrailingZeroRows(result);
    }
}
