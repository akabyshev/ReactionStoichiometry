namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

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

    private sealed record ChargeFormattingRule(String Pattern, String Readable);

    private Matrix<Double> GetCompositionMatrix()
    {
        var elements = Regex.Matches(Skeletal, ELEMENT_SYMBOL).Select(static m => m.Value).Union(new[] { "Qn", "Qp", "{e}" }).ToList();

        var data = new SpecialMatrixWritableDouble(elements.Count, _entities.Count);

        for (var r = 0; r < elements.Count; r++)
        {
            Regex regex = new(ELEMENT_TEMPLATE.Replace("X", elements[r]));

            for (var c = 0; c < _entities.Count; c++)
            {
                data[r, c] += regex.Matches(UnfoldEntity(_entities[c])).Sum(static match => Double.Parse(match.Groups[1].Value));
            }
        }

        foreach (var i in Enumerable.Range(0, _entities.Count))
        {
            foreach (var rule in new[] { new ChargeFormattingRule(@"Qn(\d*)$", "{$1-}"), new ChargeFormattingRule(@"Qp(\d*)$", "{$1+}") })
            {
                _entities[i] = Regex.Replace(_entities[i], rule.Pattern, rule.Readable);
            }
            data[elements.IndexOf("{e}"), i] = data[elements.IndexOf("Qp"), i] - data[elements.IndexOf("Qn"), i];
            (data[elements.IndexOf("Qp"), i], data[elements.IndexOf("Qn"), i]) = (0, 0);
        }

        return data.ToMatrixWithoutZeroRows();
    }
}