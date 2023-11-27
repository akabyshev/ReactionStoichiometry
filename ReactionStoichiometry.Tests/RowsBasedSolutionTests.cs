namespace ReactionStoichiometry.Tests
{
    public sealed class RowsBasedSolutionTests
    {
        [Fact]
        public void RBS_InstanceSample()
        {
            const String eq = "Fe2(SO4)3+PrTlTe3+H3PO4=Fe0.996(H2PO4)2H2O+Tl1.987(SO3)3+Pr1.998(SO4)3+Te2O3+P2O5+H2S";
            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.RowsBasedSolution.Success);
            Assert.NotNull(equation.RowsBasedSolution.InstanceSample);
            Assert.True(equation.RowsBasedSolution.FreeCoefficientIndices is { Count: 1 });
            Assert.Equal(expected: 14845224399, equation.RowsBasedSolution.InstanceSample[equation.RowsBasedSolution.FreeCoefficientIndices[index: 0]]);
        }

        [Fact]
        public void RBS_Multi_Simple()
        {
            const String eq = "TiO2 + C + Cl2 = TiCl4 + CO + CO2";
            const String sln =
                "x01·TiO2 + x02·C + x03·Cl2 + x04·TiCl4 + x05·CO + x06·CO2 = 0 with coefficients {(-x05 - 2·x06)/2, -x05 - x06, -x05 - 2·x06, (x05 + 2·x06)/2, x05, x06}";

            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.RowsBasedSolution.Success);
            Assert.Equal(sln, equation.RowsBasedSolution.ToString(OutputFormat.Simple));

            Assert.Throws<ArgumentException>(testCode: () => _ = equation.RowsBasedSolution.Instantiate(2, 5, 3));
            Assert.Throws<AppSpecificException>(testCode: () => _ = equation.RowsBasedSolution.Instantiate(3, 2));

            Assert.Null(Record.Exception(testCode: () => _ = equation.RowsBasedSolution.Instantiate(2, 5)));
            Assert.True(equation.Validate(equation.RowsBasedSolution.Instantiate(2, 5)));
            Assert.True(equation.Validate(equation.RowsBasedSolution.Instantiate(0, 0)));
        }

        [Fact]
        public void RBS_TrueNegative_Simple()
        {
            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
            var solution = new ChemicalReactionEquation(eqUnsolvable).RowsBasedSolution;
            Assert.False(solution.Success);
            Assert.Equal(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.Simple));
            Assert.Contains(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.DetailedMultiline));
            Assert.Null(solution.AlgebraicExpressions);
            Assert.Null(solution.InstanceSample);
            Assert.Null(solution.FreeCoefficientIndices);
        }

        [Fact]
        public void AlgebraicExpressionForCoefficient_Simple()
        {
            var solution = new ChemicalReactionEquation(equationString: "H2+O2+Na=H2O").RowsBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.AlgebraicExpressions);
            Assert.Equal(expected: "x01 = -x04", solution.AlgebraicExpressions[index: 0]);
            Assert.Equal(expected: "x02 = -x04/2", solution.AlgebraicExpressions[index: 1]);
            Assert.Equal(expected: "x03 = 0", solution.AlgebraicExpressions[index: 2]);
            Assert.Equal(expected: "x04", solution.AlgebraicExpressions[index: 3]);
        }
    }
}
