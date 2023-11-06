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

    private Matrix<Double> GetCompositionMatrix()
    {
        var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
        var elements = Regex.Matches(Skeletal, ELEMENT_SYMBOL).Select(static m => m.Value).Except(pseudoElementsOfCharge).ToList();
        elements.AddRange(pseudoElementsOfCharge); // it is important to have those as trailing rows of the matrix

        var data = new Double[elements.Count, _entities.Count];

        for (var r = 0; r < elements.Count; r++)
        {
            Regex regex = new(ELEMENT_TEMPLATE.Replace("X", elements[r]));

            for (var c = 0; c < _entities.Count; c++)
            {
                data[r, c] += regex.Matches(UnfoldEntity(_entities[c])).Sum(static match => Double.Parse(match.Groups[1].Value));
            }
        }

        var (indexE, indexQn, indexQp) = (elements.IndexOf("{e}"), elements.IndexOf("Qn"), elements.IndexOf("Qp"));
        for (var c = 0; c < _entities.Count; c++)
        {
            data[indexE, c] = -data[indexQn, c] + data[indexQp, c];
            data[indexQn, c] = 0;
            data[indexQp, c] = 0;
        }

        return Matrix<Double>.Build.DenseOfArray(Utils.WithoutTrailingZeroRows(data, Utils.IsZeroDouble));
    }
}

//might use some day
//foreach (var rule in new[] { new { Pseudoelement = "Qn", Sign = "-" }, new { Pseudoelement = "Qp", Sign = "+" } })
//{
//    _entities[i] = Regex.Replace(_entities[i], rule.Pseudoelement + @"(\d*)$", "{" + "$1" + rule.Sign + "}");
//}