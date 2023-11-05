namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;
using System.Text.RegularExpressions;

internal sealed partial class ChemicalReactionEquation : IChemicalEntityList
{
    private readonly List<String> _elements;
    private readonly List<String> _entities;
    internal readonly Matrix<Double> CompositionMatrix;

    internal readonly String Skeletal;
    internal Int32 OriginalReactantsCount => Skeletal.Split('=')[0].Count(static c => c == '+');

    internal ChemicalReactionEquation(String s)
    {
        if (!SeemsFine(s)) throw new ArgumentException("Invalid string");

        Skeletal = s;
        _entities = Skeletal.Split(new[] { '=', '+' }).ToList();
        _elements = Regex.Matches(Skeletal, ELEMENT_SYMBOL).Select(static m => m.Value).Union(new[] { "Qn", "Qp", "{e}" }).ToList();

        CompositionMatrix = Matrix<Double>.Build.Dense(_elements.Count, _entities.Count);
        FillCompositionMatrix(ref CompositionMatrix);
    }

    internal String AssembleEquationString<T>(T[] vector, Func<T, Boolean> mustInclude, Func<T, String> toString, Func<Int32, T, Boolean> isReactant)
    {
        if (vector.Length != EntitiesCount) throw new ArgumentOutOfRangeException(nameof(vector), "Array size mismatch");

        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < EntitiesCount; i++)
        {
            if (mustInclude(vector[i])) (isReactant(i, vector[i]) ? l : r).Add(toString(vector[i]) + GetEntity(i));
        }

        if (l.Count == 0 || r.Count == 0) return "Invalid coefficients";

        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    internal IEnumerable<String> MatrixAsStrings()
    {
        var result = new List<String>();
        result.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", CompositionMatrix.ToArray(), GetEntity, i=> _elements[i]));
        result.Add(
            $"RxC: {CompositionMatrix.RowCount}x{CompositionMatrix.ColumnCount}, rank = {CompositionMatrix.Rank()}, nullity = {CompositionMatrix.Nullity()}");

        return result;
    }

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => _entities.Count;
    public String GetEntity(Int32 i) => _entities[i];
    #endregion
}