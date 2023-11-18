//using System.Numerics;

//namespace ReactionStoichiometry.Tests
//{
//    public sealed class BalancerTests
//    {
//        [Fact]
//        public void Balancer_Ctor()
//        {
//            Assert.Throws<ArgumentException>(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2+O2=H2O:"));
//            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2 + O2 = H2O")));
//            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2+O2=H2O")));
//        }

//        [Fact]
//        public void Generalized_GuessForSingleFreeVar()
//        {
//            const String eq = "Fe2(SO4)3+PrTlTe3+H3PO4=Fe0.996(H2PO4)2H2O+Tl1.987(SO3)3+Pr1.998(SO4)3+Te2O3+P2O5+H2S";
//            var equation = new ChemicalReactionEquation(eq);
//            Assert.Null(equation.GuessedSimplestSolution.singleFreeVarValue);
//            Assert.Null(equation.GuessedSimplestSolution.balancedEquation);
//            Assert.True(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//            Assert.Equal(expected: 14845224399, equation.GuessedSimplestSolution.singleFreeVarValue);
//        }

//        [Fact]
//        public void Generalized_NoGuessForMultipleFreeVar()
//        {
//            const String eq = "CO+CO2+H2=CH4+H2O";
//            var equation = new ChemicalReactionEquation(eq);
//            Assert.True(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//            Assert.Null(equation.GuessedSimplestSolution.singleFreeVarValue);
//            Assert.Null(equation.GuessedSimplestSolution.balancedEquation);
//        }

//        [Fact]
//        public void Instantiation_CSV()
//        {
//            using StreamReader reader = new(path: @".\TestInstantiation.csv");
//            while (reader.ReadLine() is { } line)
//            {
//                if (line.StartsWith(value: '#') || line.Length == 0)
//                {
//                    continue;
//                }

//                var parts = line.Split(separator: '\t');
//                var eq = parts[0];

//                var equation = new ChemicalReactionEquation(eq);
//                var inverseBased = equation.GetSolution(ChemicalReactionEquation.SolutionTypes.InverseBased) as SolutionInverseBased;
//                var generalized = equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized) as SolutionGeneralized;

//                Assert.True(inverseBased.Success);
//                Assert.True(generalized.Success);

//                var instances = parts[1]
//                                .Split(separator: ';')
//                                .Select(StringOperations.GetParametersFromString)
//                                .Select(selector: parametersSet =>
//                                                      equation.EquationWithIntegerCoefficients(equation.Instantiate(parametersSet)));

//                Assert.Equal(inverseBased.ToString(OutputFormat.Multiline), String.Join(Environment.NewLine, instances));
//            }
//        }

//        [Fact]
//        public void InverseBased_ValidateSolution_Batch()
//        {
//            using StreamReader reader = new(path: @".\70_from_the_book.txt");

//            var detectorUniqueSolutionSetObtainedOnce = false;
//            var detectorTwoSetSolutionObtainedOnce = false;

//            while (reader.ReadLine() is { } line)
//            {
//                var equation = new ChemicalReactionEquation(line);
//                Assert.Throws<InvalidOperationException>(testCode: () => { _ = equation.SolutionSets; });
//                Assert.True(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//                Assert.NotNull(equation.SolutionSets);
//                Assert.NotEmpty(equation.SolutionSets);

//                detectorUniqueSolutionSetObtainedOnce = detectorUniqueSolutionSetObtainedOnce || equation.SolutionSets.Count == 1;
//                detectorTwoSetSolutionObtainedOnce = detectorTwoSetSolutionObtainedOnce || equation.SolutionSets.Count == 2;

//                foreach (var coefficients in equation.SolutionSets)
//                {
//                    Assert.True(equation.Validate(coefficients));
//                }
//            }

//            Assert.True(detectorUniqueSolutionSetObtainedOnce);
//            Assert.True(detectorTwoSetSolutionObtainedOnce);
//        }

//        [Fact]
//        public void InverseBased_TruePositive_Simple()
//        {
//            const String eq = "H2 + O2 = H2O";
//            var equation = new ChemicalReactionEquation(eq);
//            Assert.Throws<InvalidOperationException>(testCode: () => { _ = equation.SolutionSets[index: 0]; });
//            Assert.Equal(GlobalConstants.FAILURE_MARK, equation.ToString(OutputFormat.Simple));

//            Assert.True(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//            Assert.Null(Record.Exception(testCode: () => _ = equation.SolutionSets[index: 0]));
//            Assert.Equal(expected: "a·H2 + b·O2 + c·H2O = 0 with coefficients {-2, -1, 2}", equation.ToString(OutputFormat.Simple));
//        }

//        [Fact]
//        public void Generalized_ValidateSolution_Simple()
//        {
//            var equation = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
//            Assert.True(equation.Validate(new BigInteger[] { -2, -1, 2 }));
//            Assert.True(equation.Validate(new BigInteger[] { -4, -2, 4 }));
//            Assert.False(equation.Validate(new BigInteger[] { -10, 7, -3 }));
//            Assert.Throws<ArgumentException>(testCode: () => _ = equation.Validate(new BigInteger[] { -2, -1, 2, 2 }));
//        }

//        [Fact]
//        public void Generalized_Multi_Simple()
//        {
//            const String eq = "TiO2 + C + Cl2 = TiCl4 + CO + CO2";
//            const String sln = "a·TiO2 + b·C + c·Cl2 + d·TiCl4 + e·CO + f·CO2 = 0 with coefficients {(-e - 2·f)/2, -e - f, -e - 2·f, (e + 2·f)/2, e, f}";

//            var equation = new ChemicalReactionEquation(eq);
//            Assert.Equal(GlobalConstants.FAILURE_MARK, equation.ToString(OutputFormat.Simple));
//            Assert.Contains(GlobalConstants.FAILURE_MARK, equation.ToString(OutputFormat.DetailedMultiline));

//            Assert.True(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//            Assert.Equal(sln, equation.ToString(OutputFormat.Simple));

//            Assert.Throws<ArgumentException>(testCode: () => _ = equation.Instantiate(new BigInteger[] { 2, 5, 3 }));
//            Assert.Throws<AppSpecificException>(testCode: () => _ = equation.Instantiate(new BigInteger[] { 3, 2 }));

//            Assert.Null(Record.Exception(testCode: () => _ = equation.Instantiate(new BigInteger[] { 2, 5 })));
//            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 2, 5 })));

//            Assert.True(equation.Validate(equation.Instantiate(new BigInteger[] { 0, 0 })));
//        }

//        [Fact]
//        public void Generalized_TrueNegative_Simple()
//        {
//            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
//            var equation = new ChemicalReactionEquation(eqUnsolvable);
//            Assert.False(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//            Assert.Equal(GlobalConstants.FAILURE_MARK, equation.ToString(OutputFormat.Simple));
//            Assert.Contains(GlobalConstants.FAILURE_MARK, equation.ToString(OutputFormat.DetailedMultiline));
//        }

//        [Fact]
//        public void InverseBased_FalseNegative_Simple()
//        {
//            const String eqInverseBasedCantSolve = "O2+O3+Na+Cl2=NaCl";
//            var inverseBased = new ChemicalReactionEquation(eqInverseBasedCantSolve);
//            Assert.Equal(GlobalConstants.FAILURE_MARK, inverseBased.ToString(OutputFormat.Simple));
//            Assert.Contains(GlobalConstants.FAILURE_MARK, inverseBased.ToString(OutputFormat.DetailedMultiline));
//            Assert.False(inverseBased.Balance());
//            Assert.Equal(GlobalConstants.FAILURE_MARK, inverseBased.ToString(OutputFormat.Simple));
//            Assert.Contains(GlobalConstants.FAILURE_MARK, inverseBased.ToString(OutputFormat.DetailedMultiline));

//            var generalized = new ChemicalReactionEquation(eqInverseBasedCantSolve);
//            Assert.True(generalized.Balance());

//            Assert.True(inverseBased.Validate(generalized.Instantiate(new BigInteger[] { 0, 0 })));
//            Assert.Throws<AppSpecificException>(
//                testCode: () => inverseBased.EquationWithIntegerCoefficients(generalized.Instantiate(new BigInteger[] { 0, 0 })));

//            Assert.True(inverseBased.Validate(generalized.Instantiate(new BigInteger[] { 2, 0 })));
//            Assert.Equal(expected: "3·O2 = 2·O3", inverseBased.EquationWithIntegerCoefficients(generalized.Instantiate(new BigInteger[] { 2, 0 })));

//            Assert.True(inverseBased.Validate(generalized.Instantiate(new BigInteger[] { 0, 2 })));
//            Assert.Equal(expected: "2·Na + Cl2 = 2·NaCl"
//                       , inverseBased.EquationWithIntegerCoefficients(generalized.Instantiate(new BigInteger[] { 0, 2 })));
//        }

//        [Fact]
//        public void AlgebraicExpressionForCoefficient_Simple()
//        {
//            var equation = new ChemicalReactionEquation(equationString: "H2+O2+Na=H2O");
//            var solution = equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized) as SolutionGeneralized;
//            Assert.Throws<InvalidOperationException>(testCode: () => { _ = solution.AlgebraicExpressionForCoefficient(index: 0); });
//            Assert.Equal(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.Multiline));

//            Assert.True(solution.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);
//            Assert.Equal(expected: "-d", solution.AlgebraicExpressionForCoefficient(index: 0));
//            Assert.Equal(expected: "-d/2", solution.AlgebraicExpressionForCoefficient(index: 1));
//            Assert.Equal(expected: "0", solution.AlgebraicExpressionForCoefficient(index: 2));
//            Assert.Null(solution.AlgebraicExpressionForCoefficient(index: 3));
//        }

//        [Fact]
//        public void ToString_Simple()
//        {
//            var equation = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
//            Assert.True(equation.GetSolution(ChemicalReactionEquation.SolutionTypes.Generalized).Success);

//            var balancer2 = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
//            Assert.True(balancer2.Balance());

//            Assert.NotEqual(equation.ToString(OutputFormat.DetailedMultiline), balancer2.ToString(OutputFormat.DetailedMultiline));
//            Assert.NotEqual(equation.ToString(OutputFormat.Simple), balancer2.ToString(OutputFormat.Simple));

//            Assert.Throws<ArgumentOutOfRangeException>(testCode: () => { _ = balancer2.ToString((OutputFormat)15); });
//        }
//    }
//}


