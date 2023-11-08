namespace ReactionStoichiometry.TDD;

internal static class TestBasicParsing
{
    internal static Boolean Run()
    {
        const String inputFilePath = @"..\..\..\data\BasicParserTests.txt";
        if (!File.Exists(inputFilePath)) return false;

        using StreamReader reader = new(inputFilePath);
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith("#") || line.Length == 0) continue;
            var parts = line.Split("\t");
            Utils.AssertStringsAreEqual(StringOperations.UnfoldSubstance(parts[0]), parts[1]);
        }

        return true;
    }
}
