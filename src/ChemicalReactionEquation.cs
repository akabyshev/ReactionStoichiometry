namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

internal sealed class ChemicalReactionEquation : IChemicalEntityList
{
    private readonly List<String> _elements = new();
    private readonly List<String> _entities = new();

    public readonly String Skeletal;

    public ChemicalReactionEquation(String s)
    {
        Skeletal = s.Replace(" ", "");

        var chargeSymbols = new[] { "Qn", "Qp" };
        _elements.AddRange(Regex.Matches(Skeletal, Parsing.ELEMENT_SYMBOL).Select(static m => m.Value).Concat(chargeSymbols).Distinct());
        _elements.Add("{e}");
    }

    public Int32 ReactantsCount => Skeletal.Split('=')[0].Split('+').Length;

    #region IChemicalEntityList Members

    public Int32 EntitiesCount => _entities.Count;
    public String GetEntity(Int32 i) => _entities[i];

    #endregion

    public String GetElement(Int32 i) => _elements[i];

    public Matrix<Double> Parse()
    {
        _entities.AddRange(Regex.Split(Skeletal, Parsing.DIVIDER_CHARS));

        var resultMatrix = Matrix<Double>.Build.Dense(_elements.Count, EntitiesCount);
        for (var r = 0; r < _elements.Count; r++)
        {
            Regex regex = new(Parsing.ELEMENT_TEMPLATE.Replace("X", _elements[r]));

            for (var c = 0; c < EntitiesCount; c++)
            {
                var s = Parsing.Unfold(_entities[c]);
                resultMatrix[r, c] += regex.Matches(s).Sum(static match => Double.Parse(match.Groups[1].Value));
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

        var totalCharge = resultMatrix.Row(_elements.IndexOf("Qp")) - resultMatrix.Row(_elements.IndexOf("Qn"));
        resultMatrix.SetRow(_elements.IndexOf("{e}"), totalCharge);

        if (!totalCharge.Any(Utils.IsNonZeroDouble))
        {
            resultMatrix = resultMatrix.RemoveRow(_elements.IndexOf("{e}"));
            _elements.Remove("{e}");
        }

        resultMatrix = resultMatrix.RemoveRow(_elements.IndexOf("Qn"));
        _elements.Remove("Qn");
        resultMatrix = resultMatrix.RemoveRow(_elements.IndexOf("Qp"));
        _elements.Remove("Qp");


        return resultMatrix;
    }
}