namespace ReactionStoichiometry.Tests
{
    public sealed class PermutationsTests
    {
        [Fact]
        public void Permutations_Simple()
        {
            Assert.Equal(expected: 25, Helpers.GeneratePermutations(length: 2, maxValue: 5).Count);
            Assert.Equal(expected: 100, Helpers.GeneratePermutations(length: 2, maxValue: 10).Count);
            Assert.Equal(expected: 32, Helpers.GeneratePermutations(length: 5, maxValue: 2).Count);
        }
    }
}
