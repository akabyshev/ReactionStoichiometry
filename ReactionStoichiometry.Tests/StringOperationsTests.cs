namespace ReactionStoichiometry.Tests
{
    public sealed class StringOperationsTests
    {
        [Fact]
        public void UnfoldSubstance_CSV()
        {
            using StreamReader reader = new(path: @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.Tests\TestBasicParsing.csv");
            while (reader.ReadLine() is { } line)
            {
                var parts = line.Split(separator: ',');
                Assert.Equal(StringOperations.UnfoldSubstance(parts[0]), parts[1]);
            }
        }

        [Fact]
        public void StringLooksLikeChemicalReactionEquation_Simple()
        {
            Assert.False("C6H5COOH + O2 -> CO2 + H2O".LooksLikeChemicalReactionEquation());
            Assert.True("C6H5COOH + O2 = CO2 + H2O".LooksLikeChemicalReactionEquation());
            Assert.True("   C6H5COOH +     O2 = CO2 +  H2O    ".LooksLikeChemicalReactionEquation());
            Assert.True(@"C6H5COOH+O2=CO2+H2O".LooksLikeChemicalReactionEquation());
        }

        [Fact]
        public void AssembleEquationString_Simple()
        {
            // ReSharper disable once StringLiteralTypo
            var letters = "abcdefg".ToCharArray().Select(static c => c.ToString()).ToList();
            var numbers = "1234567".ToCharArray();

            Assert.Equal(expected: "1·a + 3·c + 5·e + 7·g = 2·b + 4·d + 6·f"
                       , StringOperations.AssembleEquationString(letters
                                                               , numbers
                                                               , omit: static _ => false
                                                               , adapter: static c => c.ToString()
                                                               , predicateGoesToRHS: static c => Convert.ToInt16(c) % 2 == 0
                                                               , allowEmptyRHS: false));
            Assert.Equal(expected: "1·a + 3·c + 5·e + 7·g = 0"
                       , StringOperations.AssembleEquationString(letters
                                                               , numbers
                                                               , omit: static c => Convert.ToInt16(c) % 2 == 0
                                                               , adapter: static c => c.ToString()
                                                               , predicateGoesToRHS: static _ => false
                                                               , allowEmptyRHS: true));
            Assert.Throws<InvalidOperationException>(() => StringOperations.AssembleEquationString(letters
                                                                                                 , numbers
                                                                                                 , omit: static c => Convert.ToInt16(c) % 2 == 0
                                                                                                 , adapter: static c => c.ToString()
                                                                                                 , predicateGoesToRHS: static _ => false
                                                                                                 , allowEmptyRHS: false));
            Assert.Throws<InvalidOperationException>(static () => StringOperations.AssembleEquationString(new List<String>()
                                                                                                        , new List<String>()
                                                                                                        , omit: static _ => false
                                                                                                        , adapter: static s => s
                                                                                                        , predicateGoesToRHS: static _ => false
                                                                                                        , allowEmptyRHS: true));
        }
    }
}