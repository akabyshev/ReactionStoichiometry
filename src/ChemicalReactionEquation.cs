namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

internal sealed partial class ChemicalReactionEquation : IChemicalEntityList
{
    private readonly List<String> _entities;
    internal readonly Matrix<Double> CompositionMatrix;
    internal readonly Int32 OriginalReactantsCount;

    internal readonly String Skeletal;

    internal ChemicalReactionEquation(String s)
    {
        if (!SeemsFine(s)) throw new ArgumentException("Invalid string");

        Skeletal = s;
        OriginalReactantsCount = 1 + Skeletal.Split('=')[0].Count(static c => c == '+');
        _entities = Skeletal.Split('=', '+').ToList();
        CompositionMatrix = GetCompositionMatrix();
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
        result.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", CompositionMatrix.ToArray(), GetEntity));
        result.Add(
            $"RxC: {CompositionMatrix.RowCount}x{CompositionMatrix.ColumnCount}, rank = {CompositionMatrix.Rank()}, nullity = {CompositionMatrix.Nullity()}");

        return result;
    }

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => _entities.Count;
    public String GetEntity(Int32 i) => _entities[i];
    #endregion
}