namespace ReactionStoichiometry;

internal class BasicTest
{
    protected const String IGNORED_LINE_MARK = "#";
    protected const String CHAR_TAB = "\t";

    protected static String ConstructPath(String classname, String? optional = null) =>
        @$"..\..\..\testdata\{classname + (optional == null ? String.Empty : '-' + optional)}.txt";

    protected static void AssertStringsAreEqual(String lhs, String rhs)
    {
        if (lhs != rhs)
            throw new Exception($"{lhs} is not equal to {rhs}");
    }
}
