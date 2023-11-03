namespace ReactionStoichiometry;

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
    private const String DIVIDER_CHARS = @"\+|=";
    private const String SKELETAL_STRUCTURE = @$"^(?:{ENTITY_ALPHABET}\+)*{ENTITY_ALPHABET}={ENTITY_ALPHABET}(?:\+{ENTITY_ALPHABET})*$";
    private const String ELEMENT_TEMPLATE = @"X(\d+(\.\d+)*)";

    public static Boolean SeemsFine(String s) => Regex.IsMatch(s, SKELETAL_STRUCTURE);

    internal static String Unfold(in String s)
    {
        var result = s;

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
}