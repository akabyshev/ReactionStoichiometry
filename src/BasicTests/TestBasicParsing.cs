namespace ReactionStoichiometry;

internal class TestBasicParsing : BasicTest
{
    internal static void Run()
    {
        using StreamReader reader = new(ConstructPath(nameof(TestBasicParsing)));
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                continue;
            var parts = line.Split(CHAR_TAB);
            AssertStringsAreEqual(StringOperations.UnfoldSubstance(parts[0]), parts[1]);
        }
    }
}
