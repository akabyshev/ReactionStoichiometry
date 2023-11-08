namespace ReactionStoichiometry;

using System.Text.RegularExpressions;
using Rationals;

internal sealed partial class ChemicalReactionEquation
{
    private const String OPENING_PARENTHESIS = @"\(";
    private const String CLOSING_PARENTHESIS = @"\)";
    private const String ELEMENT_SYMBOL = "[A-Z][a-z]|[A-Z]";
    private const String NO_INDEX_CLOSING_PARENTHESIS = @$"{CLOSING_PARENTHESIS}(?!\d)";
    private const String NO_INDEX_ELEMENT = $"({ELEMENT_SYMBOL})({ELEMENT_SYMBOL}|{OPENING_PARENTHESIS}|{CLOSING_PARENTHESIS}|$)";

    private const String INNERMOST_PARENTHESES_WITH_INDEX =
        @$"{OPENING_PARENTHESIS}([^{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+){CLOSING_PARENTHESIS}(\d+)";

    private const String SUBSTANCE_ALPHABET = @$"[A-Za-z0-9\.{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+";
    private const String SKELETAL_STRUCTURE = @$"^(?:{SUBSTANCE_ALPHABET}\+)*{SUBSTANCE_ALPHABET}={SUBSTANCE_ALPHABET}(?:\+{SUBSTANCE_ALPHABET})*$";
    private const String ELEMENT_TEMPLATE = @"X(\d+(?:\.\d+)*)";

    internal static Boolean SeemsFine(String s) => Regex.IsMatch(s, SKELETAL_STRUCTURE);

    internal static String UnfoldSubstance(in String substance)
    {
        var result = substance;

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

    private Rational[,] GetCompositionMatrix()
    {
        var pseudoElementsOfCharge = new[] { "{e}", "Qn", "Qp" };
        var elements = Regex.Matches(Skeletal, ELEMENT_SYMBOL).Select(static m => m.Value).Except(pseudoElementsOfCharge).ToList();
        elements.AddRange(pseudoElementsOfCharge); // it is important to have those as trailing rows of the matrix

        var result = new Rational[elements.Count, _substances.Count];
        for (var r = 0; r < elements.Count; r++)
            for (var c = 0; c < _substances.Count; c++)
                result[r, c] = 0;

        for (var r = 0; r < elements.Count; r++)
        {
            Regex regex = new(ELEMENT_TEMPLATE.Replace("X", elements[r]));

            for (var c = 0; c < _substances.Count; c++)
            {
                var matches = regex.Matches(UnfoldSubstance(_substances[c]));

                Rational sum = 0;
                for (var i = 0; i < matches.Count; i++)
                {
                    var match = matches[i];
                    sum += Rational.ParseDecimal(match.Groups[1].Value);
                }
                result[r, c] += sum;
            }
        }

        var (indexE, indexQn, indexQp) = (elements.IndexOf("{e}"), elements.IndexOf("Qn"), elements.IndexOf("Qp"));
        for (var c = 0; c < _substances.Count; c++)
        {
            result[indexE, c] = -result[indexQn, c] + result[indexQp, c];
            result[indexQn, c] = 0;
            result[indexQp, c] = 0;
        }

        return Utils.WithoutTrailingZeroRows(result);
    }
}

//might use some day
//foreach (var rule in new[] { new { Pseudoelement = "Qn", Sign = "-" }, new { Pseudoelement = "Qp", Sign = "+" } })
//{
//    _entities[i] = Regex.Replace(_entities[i], rule.Pseudoelement + @"(\d*)$", "{" + "$1" + rule.Sign + "}");
//}
