namespace ReactionStoichiometry
{
    internal static class REGEX
    {
        public const string MinimalSkeletalStructure = @"^.+\+.+=.+$";
        public const string ElementSymbol = @"[A-Z][a-z]|[A-Z]";
        public const string ElementNoIndex = @"([A-Z][a-z]|[A-Z])([A-Z][a-z]|[A-Z]|\(|\)|$)";
        public const string ClosingParenthesisNoIndex = @"\)(?!\d)";
        public const string InnermostParenthesesIndexed = @"\(([^\(\)]+)\)(\d+)";

        public const string AllowedDividers = @"\+|=";
        public const string ElementTemplate = @"X(\d+(\.\d+)*)";
    }
}
