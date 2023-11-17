namespace ReactionStoichiometry.Tests
{
    public sealed class StringOperationsTests
    {
        [Fact]
        public void UnfoldSubstance_CSV()
        {
            using StreamReader reader = new(path: @".\TestBasicParsing.csv");
            while (reader.ReadLine() is { } line)
            {
                var parts = line.Split(separator: ',');
                Assert.Equal(StringOperations.UnfoldSubstance(parts[0]), parts[1]);
            }
        }

        [Fact]
        public void StringLooksLikeChemicalReactionEquation_Simple()
        {
            Assert.False(StringOperations.LooksLikeChemicalReactionEquation(equationString: @"C6H5COOH + O2 = CO2 + H2O"));
            Assert.False(StringOperations.LooksLikeChemicalReactionEquation(equationString: @"C6H5COOH+O2->CO2+H2O"));
            Assert.True(StringOperations.LooksLikeChemicalReactionEquation(equationString: @"C6H5COOH+O2=CO2+H2O"));
        }

        [Fact]
        public void AssembleEquationString_Simple()
        {
            // ReSharper disable once StringLiteralTypo
            var letters = "abcdefg".ToCharArray().Select(selector: static c => c.ToString()).ToList();
            var numbers = "1234567".ToCharArray();

            Assert.Equal(expected: "1·a + 3·c + 5·e + 7·g = 2·b + 4·d + 6·f"
                       , StringOperations.AssembleEquationString(letters
                                                               , numbers
                                                               , omitIf: static _ => false
                                                               , adapter: static c => c.ToString()
                                                               , goesToRhsIf: static c => Convert.ToInt16(c) % 2 == 0
                                                               , allowEmptyRhs: false));
            Assert.Equal(expected: "1·a + 3·c + 5·e + 7·g = 0"
                       , StringOperations.AssembleEquationString(letters
                                                               , numbers
                                                               , omitIf: static c => Convert.ToInt16(c) % 2 == 0
                                                               , adapter: static c => c.ToString()
                                                               , goesToRhsIf: static _ => false
                                                               , allowEmptyRhs: true));
            Assert.Throws<AppSpecificException>(testCode: () => StringOperations.AssembleEquationString(letters
                                                                                                      , numbers
                                                                                                      , omitIf: static c => Convert.ToInt16(c) % 2 == 0
                                                                                                      , adapter: static c => c.ToString()
                                                                                                      , goesToRhsIf: static _ => false
                                                                                                      , allowEmptyRhs: false));
            Assert.Throws<AppSpecificException>(testCode: static () => StringOperations.AssembleEquationString(new List<String>()
                                                            , new List<String>()
                                                            , omitIf: static _ => false
                                                            , adapter: static s => s
                                                            , goesToRhsIf: static _ => false
                                                            , allowEmptyRhs: true));
        }
    }
}
