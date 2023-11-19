using System.Numerics;

namespace ReactionStoichiometry.Tests
{
    public sealed class MainTests
    {
        [Fact]
        public void Balancer_Ctor()
        {
            Assert.Throws<AppSpecificException>(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2+O2=H2O:"));
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2 + O2 = H2O")));
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2+O2=H2O")));
        }

        [Fact]
        public void ToString_Simple()
        {
            var equation = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
            Assert.True(equation.GeneralizedSolution.Success);
            Assert.True(equation.InverseBasedSolution.Success);

            Assert.NotEqual(equation.GeneralizedSolution.ToString(OutputFormat.DetailedMultiline)
                          , equation.InverseBasedSolution.ToString(OutputFormat.DetailedMultiline));
            Assert.NotEqual(equation.GeneralizedSolution.ToString(OutputFormat.Simple), equation.InverseBasedSolution.ToString(OutputFormat.Simple));

            Assert.Throws<ArgumentOutOfRangeException>(testCode: () => { _ = equation.InverseBasedSolution.ToString((OutputFormat)15); });
        }

        [Fact]
        public void Generalized_GuessForSingleFreeVar()
        {
            const String eq = "Fe2(SO4)3+PrTlTe3+H3PO4=Fe0.996(H2PO4)2H2O+Tl1.987(SO3)3+Pr1.998(SO4)3+Te2O3+P2O5+H2S";
            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.GeneralizedSolution.Success);
            Assert.NotNull(equation.GeneralizedSolution.GuessedSimplestSolution);
            Assert.Single(equation.GeneralizedSolution.FreeCoefficientIndices);
            Assert.Equal(expected: 14845224399
                       , equation.GeneralizedSolution.GuessedSimplestSolution[equation.GeneralizedSolution.FreeCoefficientIndices[index: 0]]);
        }

        [Fact]
        public void Generalized_NoGuessForMultipleFreeVar()
        {
            const String eq = "CO+CO2+H2=CH4+H2O";
            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.GeneralizedSolution.Success);
            Assert.Null(equation.GeneralizedSolution.GuessedSimplestSolution);
        }

        [Fact]
        public void Generalized_ValidateSolution_Simple()
        {
            var equation = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
            Assert.True(equation.Validate(new BigInteger[] { -2, -1, 2 }));
            Assert.True(equation.Validate(new BigInteger[] { -4, -2, 4 }));
            Assert.False(equation.Validate(new BigInteger[] { -10, 7, -3 }));
            Assert.Throws<ArgumentException>(testCode: () => _ = equation.Validate(new BigInteger[] { -2, -1, 2, 2 }));
        }

        [Fact]
        public void Generalized_Multi_Simple()
        {
            const String eq = "TiO2 + C + Cl2 = TiCl4 + CO + CO2";
            const String sln =
                "x01·TiO2 + x02·C + x03·Cl2 + x04·TiCl4 + x05·CO + x06·CO2 = 0 with coefficients {(-x05 - 2·x06)/2, -x05 - x06, -x05 - 2·x06, (x05 + 2·x06)/2, x05, x06}";

            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.GeneralizedSolution.Success);
            Assert.Equal(sln, equation.GeneralizedSolution.ToString(OutputFormat.Simple));

            Assert.Throws<ArgumentException>(testCode: () => _ = equation.Instantiate(new BigInteger[] { 2, 5, 3 }));
            Assert.Throws<AppSpecificException>(testCode: () => _ = equation.Instantiate(new BigInteger[] { 3, 2 }));

            Assert.Null(Record.Exception(testCode: () => _ = equation.Instantiate(new BigInteger[] { 2, 5 })));
            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 2, 5 })));
            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 0, 0 })));
        }

        [Fact]
        public void Generalized_TrueNegative_Simple()
        {
            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
            var solution = new ChemicalReactionEquation(eqUnsolvable).GeneralizedSolution;
            Assert.False(solution.Success);
            Assert.Equal(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.Simple));
            Assert.Contains(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.DetailedMultiline));
            Assert.Null(solution.AlgebraicExpressions);
            Assert.Null(solution.GuessedSimplestSolution);
            Assert.Empty(solution.FreeCoefficientIndices);
        }

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

            var generalized = equation.GeneralizedSolution;
            Assert.True(generalized.Success);

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
        public void AlgebraicExpressionForCoefficient_Simple()
        {
            var solution = new ChemicalReactionEquation(equationString: "H2+O2+Na=H2O").GeneralizedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.AlgebraicExpressions);
            Assert.Equal(expected: "x01 = -x04", solution.AlgebraicExpressions[index: 0]);
            Assert.Equal(expected: "x02 = -x04/2", solution.AlgebraicExpressions[index: 1]);
            Assert.Equal(expected: "x03 = 0", solution.AlgebraicExpressions[index: 2]);
            Assert.Equal(expected: "x04", solution.AlgebraicExpressions[index: 3]);
        }

        [Fact]
        public void Instantiation_CSV()
        {
            using StreamReader reader = new(path: @".\TestInstantiation.csv");
            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith(value: '#') || line.Length == 0)
                {
                    continue;
                }

                var parts = line.Split(separator: '\t');
                var eq = parts[0];

                var equation = new ChemicalReactionEquation(eq);
                var inverseBased = equation.InverseBasedSolution;
                var generalized = equation.GeneralizedSolution;

                Assert.True(inverseBased.Success);
                Assert.True(generalized.Success);

                var instances = parts[1]
                                .Split(separator: ';')
                                .Select(StringOperations.GetParametersFromString)
                                .Select(selector: parametersSet => equation.EquationWithIntegerCoefficients(equation.Instantiate(parametersSet)));

                Assert.Equal(inverseBased.ToString(OutputFormat.Multiline), String.Join(Environment.NewLine, instances));
            }
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
    }
}
