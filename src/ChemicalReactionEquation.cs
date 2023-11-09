namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using Rationals;

public sealed class ChemicalReactionEquation
{
    private readonly List<String> _substances;
    internal readonly Int32 OriginalReactantsCount;
    internal readonly String Skeletal;
    internal readonly Rational[,] CompositionMatrix;
    internal readonly Rational[,] CompositionMatrixReduced;
    internal readonly Int32 CompositionMatrixRank;

    internal Int32 CompositionMatrixNullity => CompositionMatrix.GetLength(dimension: 1) - CompositionMatrixRank;
    internal Int32 SubstancesCount => _substances.Count;

    internal ChemicalReactionEquation(String s)
    {
        if (!StringOperations.SeemsFine(s)) throw new ArgumentException(message: "Invalid string");
        Skeletal = s;
        OriginalReactantsCount = 1 + Skeletal.Split(separator: '=')[0].Count(predicate: static c => c == '+');

        _substances = Skeletal.Split('=', '+').ToList();
        CompositionMatrix = GetCompositionMatrix();
        CompositionMatrixReduced = RationalArrayOperations.GetSpecialForm(CompositionMatrix);
        CompositionMatrixRank = CompositionMatrixReduced.GetLength(dimension: 0);
    }

    public String GetSubstance(Int32 i) => _substances[i];

    internal IEnumerable<String> MatrixAsStrings()
    {
        var result = new List<String>();
        result.AddRange(CompositionMatrix.ToString(title: "Chemical composition matrix", GetSubstance));
        result.Add(
            $"RxC: {CompositionMatrix.RowCount()}x{CompositionMatrix.ColumnCount()}, rank = {CompositionMatrixRank}, nullity = {CompositionMatrixNullity}");

        return result;
    }

    private Rational[,] GetCompositionMatrix()
    {
        var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
        var elements = Regex.Matches(Skeletal, StringOperations.ELEMENT_SYMBOL)
                            .Select(selector: static m => m.Value)
                            .Except(pseudoElementsOfCharge)
                            .ToList();
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
            Regex regex = new(StringOperations.ELEMENT_TEMPLATE.Replace(oldValue: "X", elements[r]));

            for (var c = 0; c < _substances.Count; c++)
            {
                var matches = regex.Matches(StringOperations.UnfoldSubstance(_substances[c]));

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
        for (var c = 0; c < _substances.Count; c++)
        {
            result[indexE, c] = -result[indexQn, c] + result[indexQp, c];
            result[indexQn, c] = 0;
            result[indexQp, c] = 0;
        }

        RationalArrayOperations.CutTrailingAllZeroRows(ref result);
        return result;
    }
}
