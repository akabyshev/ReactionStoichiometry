namespace ReactionStoichiometry.Tests
{
    public sealed class CombinedTests
    {
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
                var inverseBased = equation.ColumnsBasedSolution;
                var rbs = equation.RowsBasedSolution;

                Assert.True(inverseBased.Success);
                Assert.True(rbs.Success);

                var instances = parts[1]
                                .Split(separator: ';')
                                .Select(StringOperations.GetParametersFromString)
                                .Select(selector: parametersSet => equation.EquationWithIntegerCoefficients(rbs.Instantiate(parametersSet)));

                Assert.Equal(inverseBased.ToString(OutputFormat.Multiline), String.Join(Environment.NewLine, instances));
            }
        }

        [Fact]
        public void ToString_Simple()
        {
            var equation = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
            Assert.True(equation.RowsBasedSolution.Success);
            Assert.True(equation.ColumnsBasedSolution.Success);

            Assert.NotEqual(equation.RowsBasedSolution.ToString(OutputFormat.DetailedMultiline)
                          , equation.ColumnsBasedSolution.ToString(OutputFormat.DetailedMultiline));
            Assert.NotEqual(equation.RowsBasedSolution.ToString(OutputFormat.Simple), equation.ColumnsBasedSolution.ToString(OutputFormat.Simple));

            Assert.Throws<ArgumentOutOfRangeException>(testCode: () => { _ = equation.ColumnsBasedSolution.ToString((OutputFormat)15); });
        }
    }
}
