namespace ReactionStoichiometry;

internal class BasicTest
{
    protected const String IGNORED_LINE_MARK = "#";
    protected const String CHAR_TAB = "\t";

    protected static String ConstructPath(String filename, String? str2 = null) =>
        @$"..\..\..\testdata\{filename + (str2 == null ? String.Empty : '-' + str2)}.txt";

    protected static void AssertStringsAreEqual(String lhs, String rhs)
    {
        if (lhs != rhs)
            throw new Exception($"{lhs} is not equal to {rhs}");
    }
}
