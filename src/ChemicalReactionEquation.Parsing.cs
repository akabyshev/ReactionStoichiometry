namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;
using System.Text.RegularExpressions;

internal sealed partial class ChemicalReactionEquation
{
    private const String OPENING_PARENTHESIS = @"\(";
    private const String CLOSING_PARENTHESIS = @"\)";
    private const String ELEMENT_SYMBOL = "[A-Z][a-z]|[A-Z]";
    private const String NO_INDEX_CLOSING_PARENTHESIS = @$"{CLOSING_PARENTHESIS}(?!\d)";
    private const String NO_INDEX_ELEMENT = $"({ELEMENT_SYMBOL})({ELEMENT_SYMBOL}|{OPENING_PARENTHESIS}|{CLOSING_PARENTHESIS}|$)";
    private const String INNERMOST_PARENTHESES_WITH_INDEX = @$"{OPENING_PARENTHESIS}([^{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+){CLOSING_PARENTHESIS}(\d+)";
    private const String ENTITY_ALPHABET = @$"[A-Za-z0-9\.{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+";
    private const String SKELETAL_STRUCTURE = @$"^(?:{ENTITY_ALPHABET}\+)*{ENTITY_ALPHABET}={ENTITY_ALPHABET}(?:\+{ENTITY_ALPHABET})*$";
    private const String ELEMENT_TEMPLATE = @"X(\d+(?:\.\d+)*)";

    internal static Boolean SeemsFine(String s) => Regex.IsMatch(s, SKELETAL_STRUCTURE);

    internal static String UnfoldEntity(in String entity)
    {
        var result = entity;

        {
            Regex regex = new(NO_INDEX_ELEMENT);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var element = match.Groups[1].Value;
                var rest = match.Groups[2].Value;
                result = regex.Replace(result, element + "1" + rest, 1);
            }
        }
        {
            Regex regex = new(NO_INDEX_CLOSING_PARENTHESIS);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;

                result = regex.Replace(result, ")1", 1);
            }
        }
        {
            Regex regex = new(INNERMOST_PARENTHESES_WITH_INDEX);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var token = match.Groups[1].Value;
                var index = match.Groups[2].Value;

                var repeated = String.Join(String.Empty, Enumerable.Repeat(token, Int32.Parse(index)));
                result = regex.Replace(result, repeated, 1);
            }
        }

        return result;
    }

    private void FillCompositionMatrix(ref Matrix<Double> matrix)
    {
        for (var r = 0; r < _elements.Count; r++)
        {
            Regex regex = new(ELEMENT_TEMPLATE.Replace("X", _elements[r]));

            for (var c = 0; c < _entities.Count; c++)
            {
                var s = UnfoldEntity(_entities[c]);
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
}