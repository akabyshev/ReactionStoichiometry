namespace ReactionStoichiometry;

internal static class Parsing
{
    public const String MINIMAL_SKELETAL_STRUCTURE = @"^.+\+.+=.+$";
    public const String ELEMENT_SYMBOL = @"[A-Z][a-z]|[A-Z]";
    private const String ElementNoIndex = @"([A-Z][a-z]|[A-Z])([A-Z][a-z]|[A-Z]|\(|\)|$)";
    private const String ClosingParenthesisNoIndex = @"\)(?!\d)";
    private const String InnermostParenthesesIndexed = @"\(([^\(\)]+)\)(\d+)";

    public const String FRAGMENT_DIVIDERS = @"\+|=";
    public const String ELEMENT_TEMPLATE = @"X(\d+(\.\d+)*)";

    internal static String UnfoldFragment(in String fragment)
    {
        var result = fragment;

        {
            System.Text.RegularExpressions.Regex regex = new(ElementNoIndex);
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
            System.Text.RegularExpressions.Regex regex = new(ClosingParenthesisNoIndex);
            while (true)
            {
                var match = regex.Match(result);
                if (!match.Success) break;

                result = regex.Replace(result, ")1", 1);
            }
        }
        {
            System.Text.RegularExpressions.Regex regex = new(InnermostParenthesesIndexed);
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