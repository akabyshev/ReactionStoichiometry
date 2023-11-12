namespace ReactionStoichiometryCLI
{
    internal static class Common
    {
        public const String IGNORED_LINE_MARK = "#";
        public const String CHAR_TAB = "\t";

        public static String ConstructPath(String filename, String? str2 = null)
        {
            return @$"..\..\..\batchdata\{filename + (str2 == null ? String.Empty : '-' + str2)}.txt";
        }
    }
}
