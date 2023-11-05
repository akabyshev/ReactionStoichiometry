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