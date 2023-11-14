namespace ReactionStoichiometry.CLI
{
    internal static class Common
    {
        public const String CHAR_TAB = "\t";
        public const String IGNORED_LINE_MARK = "#";

        public static String ConstructPath(String filename, String? str2 = null)
        {
            return @$"D:\Solutions\ReactionStoichiometry\batchdata\{filename + (str2 == null ? String.Empty : '-' + str2)}.txt";
        }
    }
}
