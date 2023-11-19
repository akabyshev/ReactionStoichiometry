using System.Numerics;

namespace ReactionStoichiometry.Tests
{
    public sealed class InverseBasedSolutionTests
    {
        [Fact]
        public void InverseBased_TrueNegative_Simple()
        {
            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
            var solution = new ChemicalReactionEquation(eqUnsolvable).InverseBasedSolution;
            Assert.False(solution.Success);
            Assert.Equal(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.Simple));
            Assert.Contains(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.DetailedMultiline));
            Assert.Empty(solution.IndependentReactions);
            Assert.Null(solution.CombinationSample.weights);
            Assert.Null(solution.CombinationSample.resultingCoefficients);
        }

        [Fact]
        public void InverseBased_FalseNegative_Simple()
        {
            const String eqInverseBasedCantSolve = "O2+O3+Na+Cl2=NaCl";
            var equation = new ChemicalReactionEquation(eqInverseBasedCantSolve);
            var inverseBased = equation.InverseBasedSolution;
            Assert.False(inverseBased.Success);

            Assert.Equal(GlobalConstants.FAILURE_MARK, inverseBased.ToString(OutputFormat.Simple));
            Assert.Contains(GlobalConstants.FAILURE_MARK, inverseBased.ToString(OutputFormat.DetailedMultiline));

            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 0, 0 })));
            Assert.Throws<AppSpecificException>(testCode: () => equation.EquationWithIntegerCoefficients(equation.Instantiate(new BigInteger[] { 0, 0 })));

            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 2, 0 })));
            Assert.Equal(expected: "3·O2 = 2·O3", equation.EquationWithIntegerCoefficients(equation.Instantiate(new BigInteger[] { 2, 0 })));

            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 0, 2 })));
            Assert.Equal(expected: "2·Na + Cl2 = 2·NaCl", equation.EquationWithIntegerCoefficients(equation.Instantiate(new BigInteger[] { 0, 2 })));
        }

        [Fact]
        public void InverseBased_TruePositive_Simple()
        {
            const String eq = "H2 + O2 = H2O";
            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.InverseBasedSolution.Success);
            Assert.Equal(expected: "x01·H2 + x02·O2 + x03·H2O = 0 with coefficients {-2, -1, 2}", equation.InverseBasedSolution.ToString(OutputFormat.Simple));
        }

        [Fact]
        public void InverseBased_ValidateSolution_Batch()
        {
            using StreamReader reader = new(path: @".\70_from_the_book.txt");

            while (reader.ReadLine() is { } line)
            {
                var equation = new ChemicalReactionEquation(line);
                Assert.True(equation.InverseBasedSolution.Success);

                foreach (var coefficients in equation.InverseBasedSolution.IndependentReactions)
                {
                    Assert.True(equation.Validate(coefficients));
                }
            }
        }

        [Fact]
        public void CombinationsOfTwo()
        {
            var solution = new ChemicalReactionEquation(equationString: "C6H5C2H5 + O2 = C6H5OH + CO2 + H2O").InverseBasedSolution;
            Assert.True(solution.Success);
            Assert.Equal(expected: 2, solution.IndependentReactions.Count);
            Assert.NotNull(solution.CombinationSample.weights);
            Assert.Null(solution.CombinationSample.resultingCoefficients); // current implementation sets 1,2 weights
            Assert.Null(solution.GetCombinationOfIndependents(new[] { 0, 0 }));
            Assert.Null(solution.GetCombinationOfIndependents(new[] { 1, 1 })); // C6H5C2H5 is -6 and +6
            Assert.Null(solution.GetCombinationOfIndependents(new[] { 4, 5 })); // C6H5OH is -10*4 and +8*5
            Assert.NotNull(solution.GetCombinationOfIndependents(new[] { 4, 6 }));
        }

        [Fact]
        public void CombinationsOfOne()
        {
            var solution = new ChemicalReactionEquation(equationString: "K4Fe(CN)6+H2SO4+H2O=K2SO4+FeSO4+(NH4)2SO4+CO").InverseBasedSolution;
            Assert.True(solution.Success);
            Assert.Single(solution.IndependentReactions);
            Assert.Null(solution.CombinationSample.weights);
            Assert.Null(solution.CombinationSample.resultingCoefficients);
        }
    }
}
