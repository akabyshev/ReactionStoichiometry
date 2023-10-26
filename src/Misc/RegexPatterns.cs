namespace ReactionStoichiometry;

internal static class RegexPatterns
{
    public const string MINIMAL_SKELETAL_STRUCTURE = @"^.+\+.+=.+$";
    public const string ELEMENT_SYMBOL = @"[A-Z][a-z]|[A-Z]";
    public const string ELEMENT_NO_INDEX = @"([A-Z][a-z]|[A-Z])([A-Z][a-z]|[A-Z]|\(|\)|$)";
    public const string CLOSING_PARENTHESIS_NO_INDEX = @"\)(?!\d)";
    public const string INNERMOST_PARENTHESES_INDEXED = @"\(([^\(\)]+)\)(\d+)";

    public const string FRAGMENT_DIVIDERS = @"\+|=";
    public const string ELEMENT_TEMPLATE = @"X(\d+(\.\d+)*)";
}