using System.Numerics;


namespace ReactionStoichiometry.Tests
{
    public sealed class BalancerTests
    {
        [Fact]
        public void Balancer_Ctor()
        {
            Assert.Throws<ArgumentException>(() => _ = new BalancerGeneralized(equation: "H2+O2=H2O:"));
            Assert.Null(Record.Exception(() => _ = new BalancerGeneralized(equation: "H2 + O2 = H2O")));
            Assert.Null(Record.Exception(() => _ = new BalancerGeneralized(equation: "H2+O2=H2O")));
        }

        [Fact]
        public void Instantiation_CSV()
        {
            using StreamReader reader = new(path: @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.Tests\TestInstantiation.csv");
            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith(value: '#') || line.Length == 0)
                    continue;

                var parts = line.Split(separator: '\t');
                var eq = parts[0];

                var inverseBased = new BalancerInverseBased(eq);
                inverseBased.Run();

                var generalized = new BalancerGeneralized(eq);
                generalized.Run();

                var instances = parts[1]
                                .Split(separator: ';')
                                .Select(StringOperations.GetArraysFromCoefficientNotationString)
                                .Select(selector: parametersSet =>
                                                      generalized.EquationWithIntegerCoefficients(generalized.Instantiate(parametersSet)));

                Assert.Equal(inverseBased.ToString(Balancer.OutputFormat.OutcomeOnlyCommas), String.Join(separator: ",", instances));
            }
        }

        [Fact]
        public void InverseBased_ValidateSolution_Batch()
        {
            using StreamReader reader = new(path: @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometry.Tests\70_from_the_book.txt");

            var detectorUniqueSolutionSetObtainedOnce = false;
            var detectorTwoSetSolutionObtainedOnce = false;

            while (reader.ReadLine() is { } line)
            {
                var balancer = new BalancerInverseBased(line);
                Assert.Throws<InvalidOperationException>(() => { _ = balancer.SolutionSets; });
                Assert.True(balancer.Run());
                Assert.NotNull(balancer.SolutionSets);
                Assert.NotEmpty(balancer.SolutionSets);

                detectorUniqueSolutionSetObtainedOnce = detectorUniqueSolutionSetObtainedOnce || balancer.SolutionSets.Count == 1;
                detectorTwoSetSolutionObtainedOnce = detectorTwoSetSolutionObtainedOnce || balancer.SolutionSets.Count == 2;

                foreach (var coefficients in balancer.SolutionSets)
                    Assert.True(balancer.ValidateSolution(coefficients));
            }

            Assert.True(detectorUniqueSolutionSetObtainedOnce);
            Assert.True(detectorTwoSetSolutionObtainedOnce);
        }

        [Fact]
        public void InverseBased_TruePositive_Simple()
        {
            const String eq = "H2 + O2 = H2O";
            var balancer = new BalancerInverseBased(eq);
            Assert.Throws<InvalidOperationException>(() => { _ = balancer.SolutionSets[index: 0]; });
            Assert.Equal(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.SingleLine));

            Assert.True(balancer.Run());
            Assert.Null(Record.Exception(() => _ = balancer.SolutionSets[index: 0]));
            Assert.Equal(expected: "a·H2 + b·O2 + c·H2O = 0 with coefficients {-2, -1, 2}", balancer.ToString(Balancer.OutputFormat.SingleLine));
        }

        [Fact]
        public void Generalized_ValidateSolution_Simple()
        {
            var balancer = new BalancerGeneralized(equation: "H2+O2=H2O");
            Assert.True(balancer.ValidateSolution(new BigInteger[] { -2, -1, 2 }));
            Assert.True(balancer.ValidateSolution(new BigInteger[] { -4, -2, 4 }));
            Assert.False(balancer.ValidateSolution(new BigInteger[] { -10, 7, -3 }));
            Assert.Throws<ArgumentException>(() => _ = balancer.ValidateSolution(new BigInteger[] { -2, -1, 2, 2 }));
        }

        [Fact]
        public void Generalized_Multi_Simple()
        {
            const String eq = "TiO2 + C + Cl2 = TiCl4 + CO + CO2";
            const String sln =
                "a·TiO2 + b·C + c·Cl2 + d·TiCl4 + e·CO + f·CO2 = 0 with coefficients {(-e - 2·f)/2, -e - f, -e - 2·f, (e + 2·f)/2, e, f}";

            var balancer = new BalancerGeneralized(eq);
            Assert.Equal(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.SingleLine));
            Assert.Contains(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.DetailedPlain));

            Assert.True(balancer.Run());
            Assert.Equal(sln, balancer.ToString(Balancer.OutputFormat.SingleLine));

            Assert.Throws<ArgumentException>(() => _ = balancer.Instantiate(new BigInteger[] { 2, 5, 3 }));
            Assert.Throws<AppSpecificException>(() => _ = balancer.Instantiate(new BigInteger[] { 3, 2 }));


            Assert.Null(Record.Exception(() => _ = balancer.Instantiate(new BigInteger[] { 2, 5 })));
            Assert.True(balancer.ValidateSolution(balancer.Instantiate(new BigInteger[] { 2, 5 })));

            Assert.True(balancer.ValidateSolution(balancer.Instantiate(new BigInteger[] { 0, 0 })));
        }

        [Fact]
        public void Generalized_TrueNegative_Simple()
        {
            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
            var balancer = new BalancerGeneralized(eqUnsolvable);
            Assert.False(balancer.Run());
            Assert.Equal(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.SingleLine));
            Assert.Contains(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.DetailedPlain));
        }

        [Fact]
        public void InverseBased_FalseNegative_Simple()
        {
            const String eqInverseBasedCantSolve = "O2+O3+Na+Cl2=NaCl";
            var inverseBased = new BalancerInverseBased(eqInverseBasedCantSolve);
            Assert.Equal(GlobalConstants.FAILURE_MARK, inverseBased.ToString(Balancer.OutputFormat.SingleLine));
            Assert.Contains(GlobalConstants.FAILURE_MARK, inverseBased.ToString(Balancer.OutputFormat.DetailedPlain));
            Assert.False(inverseBased.Run());
            Assert.Equal(GlobalConstants.FAILURE_MARK, inverseBased.ToString(Balancer.OutputFormat.SingleLine));
            Assert.Contains(GlobalConstants.FAILURE_MARK, inverseBased.ToString(Balancer.OutputFormat.DetailedPlain));
            Assert.Contains(GlobalConstants.FAILURE_MARK, inverseBased.ToString(Balancer.OutputFormat.DetailedHtml));

            var generalized = new BalancerGeneralized(eqInverseBasedCantSolve);
            Assert.True(generalized.Run());

            Assert.True(inverseBased.ValidateSolution(generalized.Instantiate(new BigInteger[] { 0, 0 })));
            Assert.Throws<InvalidOperationException>(
                () => inverseBased.EquationWithIntegerCoefficients(generalized.Instantiate(new BigInteger[] { 0, 0 })));

            Assert.True(inverseBased.ValidateSolution(generalized.Instantiate(new BigInteger[] { 2, 0 })));
            Assert.Equal(expected: "3·O2 = 2·O3", inverseBased.EquationWithIntegerCoefficients(generalized.Instantiate(new BigInteger[] { 2, 0 })));

            Assert.True(inverseBased.ValidateSolution(generalized.Instantiate(new BigInteger[] { 0, 2 })));
            Assert.Equal(expected: "2·Na + Cl2 = 2·NaCl"
                       , inverseBased.EquationWithIntegerCoefficients(generalized.Instantiate(new BigInteger[] { 0, 2 })));
        }

        [Fact]
        public void AlgebraicExpressionForCoefficient_Simple()
        {
            var balancer = new BalancerGeneralized(equation: "H2+O2+Na=H2O");
            Assert.Throws<InvalidOperationException>(() => { _ = balancer.AlgebraicExpressionForCoefficient(index: 0); });
            Assert.Equal(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.OutcomeOnlyCommas));

            Assert.True(balancer.Run());
            Assert.Equal(expected: "-d", balancer.AlgebraicExpressionForCoefficient(index: 0));
            Assert.Equal(expected: "-d/2", balancer.AlgebraicExpressionForCoefficient(index: 1));
            Assert.Equal(expected: "0", balancer.AlgebraicExpressionForCoefficient(index: 2));
            Assert.Null(balancer.AlgebraicExpressionForCoefficient(index: 3));

            Assert.Equal(expected: "a = -d,b = -d/2,c = 0,for any {d}", balancer.ToString(Balancer.OutputFormat.OutcomeOnlyCommas));
            Assert.Equal(String.Join(Environment.NewLine, "a = -d", "b = -d/2", "c = 0", "for any {d}")
                       , balancer.ToString(Balancer.OutputFormat.OutcomeOnlyNewLine));
        }

        [Fact]
        public void ToString_Simple()
        {
            var balancer1 = new BalancerGeneralized(equation: "H2+O2=H2O");
            Assert.True(balancer1.Run());

            var balancer2 = new BalancerInverseBased(equation: "H2+O2=H2O");
            Assert.True(balancer2.Run());

            Assert.NotEqual(balancer1.ToString(Balancer.OutputFormat.DetailedPlain), balancer2.ToString(Balancer.OutputFormat.DetailedPlain));
            Assert.NotEqual(balancer1.ToString(Balancer.OutputFormat.SingleLine), balancer2.ToString(Balancer.OutputFormat.SingleLine));

            Assert.Throws<ArgumentOutOfRangeException>(() => { _ = balancer2.ToString((Balancer.OutputFormat) 15); });
        }
    }
}
