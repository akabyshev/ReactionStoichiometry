namespace ReactionStoichiometry;

internal sealed partial class ChemicalReactionEquation : ISubstancesList
{
    private readonly List<String> _substances;
    internal readonly RationalMatrix CompositionMatrix;
    internal readonly Int32 OriginalReactantsCount;

    internal readonly String Skeletal;

    internal ChemicalReactionEquation(String s)
    {
        if (!SeemsFine(s)) throw new ArgumentException("Invalid string");
        Skeletal = s;
        OriginalReactantsCount = 1 + Skeletal.Split('=')[0].Count(static c => c == '+');

        _substances = Skeletal.Split('=', '+').ToList();
        CompositionMatrix = RationalMatrix.CreateInstance(GetCompositionMatrix(), static v => v);
    }

    internal IEnumerable<String> MatrixAsStrings()
    {
        var result = new List<String>();
        result.AddRange(CompositionMatrix.PrettyPrint("Chemical composition matrix", GetSubstance));
        result.Add(
            $"RxC: {CompositionMatrix.RowCount}x{CompositionMatrix.ColumnCount}, rank = {CompositionMatrix.Rank}, nullity = {CompositionMatrix.Nullity}");

        return result;
    }

    #region ISubstancesList Members
    public Int32 SubstancesCount => _substances.Count;
    public String GetSubstance(Int32 i) => _substances[i];
    #endregion
}
