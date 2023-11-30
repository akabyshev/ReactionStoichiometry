using System.Numerics;

namespace ReactionStoichiometry.Tests
{
    public sealed class ColumnsBasedSolutionTests
    {
        [Fact]
        public void TrueNegative_Simple()
        {
            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
            var solution = new ChemicalReactionEquation(eqUnsolvable).ColumnsBasedSolution;
            Assert.False(solution.Success);
            Assert.Equal(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.Simple));
            Assert.Contains(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.DetailedMultiline));
            Assert.Null(solution.IndependentSetsOfCoefficients);
            Assert.Null(solution.CombinationSample.recipe);
            Assert.Null(solution.CombinationSample.coefficients);
        }

        [Fact]
        public void TruePositive_Simple()
        {
            const String eq = "H2 + O2 = H2O";
            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.ColumnsBasedSolution.Success);
            Assert.Equal(expected: "x01·H2 + x02·O2 + x03·H2O = 0 with coefficients {-2, -1, 2}", equation.ColumnsBasedSolution.ToString(OutputFormat.Simple));
        }

        [Fact]
        public void ValidateSolution_Batch()
        {
            using StreamReader reader = new(path: @".\70_from_the_book.txt");

            while (reader.ReadLine() is { } line)
            {
                var equation = new ChemicalReactionEquation(line);
                Assert.True(equation.ColumnsBasedSolution.Success);
                Assert.NotNull(equation.ColumnsBasedSolution.IndependentSetsOfCoefficients);

                foreach (var coefficients in equation.ColumnsBasedSolution.IndependentSetsOfCoefficients)
                {
                    Assert.True(equation.Validate(coefficients));
                }
            }
        }

        [Fact]
        public void CombinationsOfTwo()
        {
            var solution = new ChemicalReactionEquation(equationString: "C6H5C2H5 + O2 = C6H5OH + CO2 + H2O").ColumnsBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Equal(expected: 2, solution.IndependentSetsOfCoefficients.Count);
            Assert.Equal(new BigInteger[] { 6, -7, -10, 12, 0 }, solution.IndependentSetsOfCoefficients[index: 0]);
            Assert.Equal(new BigInteger[] { -6, -7, 8, 0, 6 }, solution.IndependentSetsOfCoefficients[index: 1]);

            // all-zero
            Assert.Equal(new BigInteger[] { 0, 0, 0, 0, 0 }, solution.CombineIndependents(0, 0));

            // C6H5C2H5 is -6 and +6        others reduced /2
            Assert.Equal(new BigInteger[] { 0, -7, -1, 6, 3 }, solution.CombineIndependents(1, 1));

            // C6H5OH is -10*4 and +8*5     others reduced /3 :
            Assert.Equal(new BigInteger[] { -2, -21, 0, 16, 10 }, solution.CombineIndependents(4, 5));

            // non-zero                     non-reducible
            Assert.Equal(new BigInteger[] { -6, -35, 4, 24, 18 }, solution.CombineIndependents(4, 6));
        }

        [Fact]
        public void CombinationsOfOne()
        {
            var solution = new ChemicalReactionEquation(equationString: "K4Fe(CN)6+H2SO4+H2O=K2SO4+FeSO4+(NH4)2SO4+CO").ColumnsBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Single(solution.IndependentSetsOfCoefficients);
            Assert.Null(solution.CombinationSample.recipe);
            Assert.Null(solution.CombinationSample.coefficients);
        }
    }
}
