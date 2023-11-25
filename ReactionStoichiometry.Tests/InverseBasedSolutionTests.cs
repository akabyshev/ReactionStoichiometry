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
            Assert.Null(solution.IndependentSetsOfCoefficients);
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

            Assert.True(equation.Validate(equation.GeneralizedSolution.Instantiate(new BigInteger[] { 0, 0 })));
            Assert.Throws<AppSpecificException>(
                testCode: () => equation.EquationWithIntegerCoefficients(equation.GeneralizedSolution.Instantiate(new BigInteger[] { 0, 0 })));

            Assert.True(equation.Validate(equation.GeneralizedSolution.Instantiate(new BigInteger[] { 2, 0 })));
            Assert.Equal(expected: "3·O2 = 2·O3"
                       , equation.EquationWithIntegerCoefficients(equation.GeneralizedSolution.Instantiate(new BigInteger[] { 2, 0 })));

            Assert.True(equation.Validate(equation.GeneralizedSolution.Instantiate(new BigInteger[] { 0, 2 })));
            Assert.Equal(expected: "2·Na + Cl2 = 2·NaCl"
                       , equation.EquationWithIntegerCoefficients(equation.GeneralizedSolution.Instantiate(new BigInteger[] { 0, 2 })));
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
                Assert.NotNull(equation.InverseBasedSolution.IndependentSetsOfCoefficients);

                foreach (var coefficients in equation.InverseBasedSolution.IndependentSetsOfCoefficients)
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
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Equal(expected: 2, solution.IndependentSetsOfCoefficients.Count);
            Assert.Equal(expected: new BigInteger[] { 6, -7, -10, 12, 0 }, solution.IndependentSetsOfCoefficients[index: 0]);
            Assert.Equal(expected: new BigInteger[] { -6, -7, 8, 0, 6 }, solution.IndependentSetsOfCoefficients[index: 1]);

            Assert.Equal(expected: new BigInteger[] { 0, 0, 0, 0, 0 }, solution.CombineIndependents(new[] { 0, 0 }));           // all-zero
            Assert.Equal(expected: new BigInteger[] { 0, -14, -2, 12, 6 }, solution.CombineIndependents(new[] { 1, 1 }));       // C6H5C2H5 is -6 and +6
            Assert.Equal(expected: new BigInteger[] { -6, -63, 0, 48, 30 }, solution.CombineIndependents(new[] { 4, 5 }));      // C6H5OH is -10*4 and +8*5
            Assert.Equal(expected: new BigInteger[] { -12, -70, 8, 48, 36 }, solution.CombineIndependents(new[] { 4, 6 }));     // non-zero
        }

        [Fact]
        public void CombinationsOfTwo_ImplementationSpecific_CounterExample()
        {
            // weights of 1,2 must not work on the equation in this test
            var solution = new ChemicalReactionEquation(equationString: "CO+CO2+H2=CH4+H2O").InverseBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Equal(expected: 2, solution.IndependentSetsOfCoefficients.Count);
            Assert.Null(solution.CombinationSample.resultingCoefficients);
            Assert.Null(solution.CombinationSample.weights);
        }

        [Fact]
        public void CombinationsOfOne()
        {
            var solution = new ChemicalReactionEquation(equationString: "K4Fe(CN)6+H2SO4+H2O=K2SO4+FeSO4+(NH4)2SO4+CO").InverseBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Single(solution.IndependentSetsOfCoefficients);
            Assert.Null(solution.CombinationSample.weights);
            Assert.Null(solution.CombinationSample.resultingCoefficients);
        }
    }
}
