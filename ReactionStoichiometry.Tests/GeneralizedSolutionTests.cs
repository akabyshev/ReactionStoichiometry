using System.Numerics;

namespace ReactionStoichiometry.Tests
{
    public sealed class GeneralizedSolutionTests
    {
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
    }
}
