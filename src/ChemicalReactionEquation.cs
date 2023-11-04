namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

internal sealed partial class ChemicalReactionEquation : IChemicalEntityList
{
    private readonly List<String> _elements = new();
    private readonly List<String> _entities = new();

    internal readonly Int32 ReactantsCount;
    internal readonly String Skeletal;

    internal ChemicalReactionEquation(String s)
    {
        Skeletal = s;
        if (!SeemsFine(Skeletal)) throw new ArgumentException("Invalid string");
        ReactantsCount = Skeletal.Split('=')[0].Split('+').Length;

        var chargeSymbols = new[] { "Qn", "Qp" };
        _elements.AddRange(Regex.Matches(Skeletal, ELEMENT_SYMBOL).Select(static m => m.Value).Concat(chargeSymbols).Distinct());
        _elements.Add("{e}");

        _entities.AddRange(Regex.Split(Skeletal, DIVIDER_CHARS));
    }

    internal String GetElement(Int32 i) => _elements[i];

    internal void Parse(out Matrix<Double> matrix)
    {
        matrix = Matrix<Double>.Build.Dense(_elements.Count, EntitiesCount);
        for (var r = 0; r < _elements.Count; r++)
        {
            Regex regex = new(ELEMENT_TEMPLATE.Replace("X", _elements[r]));

            for (var c = 0; c < EntitiesCount; c++)
            {
                var s = Unfold(_entities[c]);
                matrix[r, c] += regex.Matches(s).Sum(static match => Double.Parse(match.Groups[1].Value));
            }
        }

        var chargeParsingRules = new[] { new[] { "Qn", @"Qn(\d*)$", "{$1-}" }, new[] { "Qp", @"Qp(\d*)$", "{$1+}" } };
        for (var i = 0; i < EntitiesCount; i++)
        {
            foreach (var chargeParsingRule in chargeParsingRules)
            {
                _entities[i] = Regex.Replace(_entities[i], chargeParsingRule[1], chargeParsingRule[2]);
            }
        }

        var totalCharge = matrix.Row(_elements.IndexOf("Qp")) - matrix.Row(_elements.IndexOf("Qn"));
        matrix.SetRow(_elements.IndexOf("{e}"), totalCharge);

        if (!totalCharge.Any(Utils.IsNonZeroDouble))
        {
            matrix = matrix.RemoveRow(_elements.IndexOf("{e}"));
            _elements.Remove("{e}");
        }

        matrix = matrix.RemoveRow(_elements.IndexOf("Qn"));
        _elements.Remove("Qn");
        matrix = matrix.RemoveRow(_elements.IndexOf("Qp"));
        _elements.Remove("Qp");
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

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => _entities.Count;
    public String GetEntity(Int32 i) => _entities[i];
    #endregion
}