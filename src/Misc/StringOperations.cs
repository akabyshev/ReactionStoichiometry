namespace ReactionStoichiometry;

using System.Text.RegularExpressions;

public static class StringOperations
{
    private const String OPENING_PARENTHESIS = @"\(";
    private const String CLOSING_PARENTHESIS = @"\)";
    internal const String ELEMENT_SYMBOL = "[A-Z][a-z]|[A-Z]";
    private const String NO_INDEX_CLOSING_PARENTHESIS = @$"{CLOSING_PARENTHESIS}(?!\d)";
    private const String NO_INDEX_ELEMENT = $"({ELEMENT_SYMBOL})({ELEMENT_SYMBOL}|{OPENING_PARENTHESIS}|{CLOSING_PARENTHESIS}|$)";

    private const String INNERMOST_PARENTHESES_WITH_INDEX =
        @$"{OPENING_PARENTHESIS}([^{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+){CLOSING_PARENTHESIS}(\d+)";

    private const String SUBSTANCE_ALPHABET = @$"[A-Za-z0-9\.{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+";
    private const String SKELETAL_STRUCTURE = @$"^(?:{SUBSTANCE_ALPHABET}\+)*{SUBSTANCE_ALPHABET}={SUBSTANCE_ALPHABET}(?:\+{SUBSTANCE_ALPHABET})*$";
    internal const String ELEMENT_TEMPLATE = @"X(\d+(?:\.\d+)*)";

    public static String UnfoldSubstance(String substance)
    {
        var result = substance;

        {
            Regex regex = new(NO_INDEX_ELEMENT);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var element = match.Groups[groupnum: 1].Value;
                var rest = match.Groups[groupnum: 2].Value;
                result = regex.Replace(result, element + "1" + rest, count: 1);
            }
        }
        {
            Regex regex = new(NO_INDEX_CLOSING_PARENTHESIS);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;

                result = regex.Replace(result, replacement: ")1", count: 1);
            }
        }
        {
            Regex regex = new(INNERMOST_PARENTHESES_WITH_INDEX);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var token = match.Groups[groupnum: 1].Value;
                var index = match.Groups[groupnum: 2].Value;

                var repeated = String.Join(String.Empty, Enumerable.Repeat(token, Int32.Parse(index)));
                result = regex.Replace(result, repeated, count: 1);
            }
        }

        return result;
    }

    internal static Boolean SeemsFine(String s) => Regex.IsMatch(s, SKELETAL_STRUCTURE);
}

//might use some day
//foreach (var rule in new[] { new { Pseudoelement = "Qn", Sign = "-" }, new { Pseudoelement = "Qp", Sign = "+" } })
//{
//    _entities[i] = Regex.Replace(_entities[i], rule.Pseudoelement + @"(\d*)$", "{" + "$1" + rule.Sign + "}");
//}
