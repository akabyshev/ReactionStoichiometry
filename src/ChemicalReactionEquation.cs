namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

internal sealed partial class ChemicalReactionEquation : ISubstancesList
{
    private readonly List<String> _substances;
    internal readonly Matrix<Double> CompositionMatrix;
    internal readonly Int32 OriginalReactantsCount;

    internal readonly String Skeletal;

    internal ChemicalReactionEquation(String s)
    {
        if (!SeemsFine(s)) throw new ArgumentException("Invalid string");
        Skeletal = s;
        OriginalReactantsCount = 1 + Skeletal.Split('=')[0].Count(static c => c == '+');

        _substances = Skeletal.Split('=', '+').ToList();
        CompositionMatrix = Matrix<Double>.Build.DenseOfArray(GetCompositionMatrix());
    }

    internal IEnumerable<String> MatrixAsStrings()
    {
        var result = new List<String>();
        result.AddRange(Utils.PrettyPrint("Chemical composition matrix", CompositionMatrix.ToArray(), GetSubstance));
        result.Add(
            $"RxC: {CompositionMatrix.RowCount}x{CompositionMatrix.ColumnCount}, rank = {CompositionMatrix.Rank()}, nullity = {CompositionMatrix.Nullity()}");

        return result;
    }

    #region ISubstancesList Members
    public Int32 SubstancesCount => _substances.Count;
    public String GetSubstance(Int32 i) => _substances[i];
    #endregion
}
