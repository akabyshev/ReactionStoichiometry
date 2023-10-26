using System.Text.RegularExpressions;

namespace ReactionStoichiometry;

internal static class Parsing
{
    public const string MINIMAL_SKELETAL_STRUCTURE = @"^.+\+.+=.+$";
    public const string ELEMENT_SYMBOL = @"[A-Z][a-z]|[A-Z]";
    private const string ElementNoIndex = @"([A-Z][a-z]|[A-Z])([A-Z][a-z]|[A-Z]|\(|\)|$)";
    private const string ClosingParenthesisNoIndex = @"\)(?!\d)";
    private const string InnermostParenthesesIndexed = @"\(([^\(\)]+)\)(\d+)";

    public const string FRAGMENT_DIVIDERS = @"\+|=";
    public const string ELEMENT_TEMPLATE = @"X(\d+(\.\d+)*)";

    internal static string UnfoldFragment(in string fragment)
    {
        var result = fragment;

        {
            Regex regex = new(ElementNoIndex);
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
            Regex regex = new(ClosingParenthesisNoIndex);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;

                result = regex.Replace(result, ")1", 1);
            }
        }
        {
            Regex regex = new(InnermostParenthesesIndexed);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;
                var token = match.Groups[1].Value;
                var index = match.Groups[2].Value;

                var repeated = string.Join("", Enumerable.Repeat(token, int.Parse(index)));
                result = regex.Replace(result, repeated, 1);
            }
        }

        return result;
    }
}