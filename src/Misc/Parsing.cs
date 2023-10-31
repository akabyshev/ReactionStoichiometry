namespace ReactionStoichiometry;

using System.Text.RegularExpressions;


internal static class Parsing
{
    public const String CRE_ALLOWED_DIVIDERS = @"\+|=";
    public const String MINIMAL_SKELETAL_STRUCTURE = @"^.+\+.+=.+$";
    public const String ELEMENT_SYMBOL = "[A-Z][a-z]|[A-Z]";
    public const String ELEMENT_TEMPLATE = @"X(\d+(\.\d+)*)";
    private const String ElementNoIndex = @"([A-Z][a-z]|[A-Z])([A-Z][a-z]|[A-Z]|\(|\)|$)";
    private const String ClosingParenthesisNoIndex = @"\)(?!\d)";
    private const String InnermostParenthesesIndexed = @"\(([^\(\)]+)\)(\d+)";

    internal static String Unfold(in String s)
    {
        var result = s;

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

                var repeated = String.Join("", Enumerable.Repeat(token, Int32.Parse(index)));
                result = regex.Replace(result, repeated, 1);
            }
        }

        return result;
    }
}