using ReactionStoichiometry;

namespace ReactionStoichiometryTests
{
    public sealed class ParserTests
    {
        [Fact]
        public void Unfold_CSV()
        {
            using StreamReader reader = new(path: @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometryTests\TestBasicParsing.csv");
            while (reader.ReadLine() is { } line)
            {
                var parts = line.Split(separator: ',');
                Assert.Equal(StringOperations.UnfoldSubstance(parts[0]), parts[1]);
            }
        }
    }
}