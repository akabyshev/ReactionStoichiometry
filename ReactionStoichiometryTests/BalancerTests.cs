using System.Numerics;
using ReactionStoichiometry;


namespace ReactionStoichiometryTests
{
    public sealed class BalancerTests
    {
        [Fact]
        public void InvalidEquations()
        {
            Assert.Throws<ArgumentException>(() => _ = new BalancerGeneralized(equation: "H2+O2=H2O:"));
            Assert.Throws<ArgumentException>(() => _ = new BalancerGeneralized(equation: "H2 + O2 = H2O"));
            Assert.Null(Record.Exception(() => _ = new BalancerGeneralized(equation: "H2+O2=H2O")));
        }

        [Fact]
        public void Instantiate_CSV()
        {
            using StreamReader reader = new(path: @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometryTests\TestInstantiation.csv");
            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith(value: '#') || line.Length == 0)
                    continue;

                var parts = line.Split(separator: '\t');
                var eq = parts[0].Replace(oldValue: " ", newValue: "");

                var inverseBased = new BalancerInverseBased(eq);
                inverseBased.Run();

                var generalized = new BalancerGeneralized(eq);
                generalized.Run();
                var instances = parts[1]
                                .Split(separator: ';')
                                .Select(selector: static s => s.Trim('(', ')').Split(separator: ',').Select(BigInteger.Parse))
                                .Select(selector: parametersSet =>
                                                      generalized.EquationWithIntegerCoefficients(generalized.Instantiate(parametersSet.ToArray())));

                Assert.Equal(inverseBased.ToString(Balancer.OutputFormat.OutcomeOnlyCommas), String.Join(separator: ",", instances));
            }
        }

        [Fact]
        public void ValidatingSolutions_Batch()
        {
            using StreamReader reader = new(path: @"D:\Solutions\ReactionStoichiometry\ReactionStoichiometryTests\70_from_the_book.txt");

            var unique_solution_set_obtained_once = false;
            var two_set_solution_obtained_once = false;

            while (reader.ReadLine() is { } line)
            {
                var balancer = new BalancerInverseBased(line);
                balancer.Run();

                unique_solution_set_obtained_once = unique_solution_set_obtained_once || balancer.SolutionSets.Count == 1;
                two_set_solution_obtained_once = two_set_solution_obtained_once || balancer.SolutionSets.Count == 2;

                foreach (var coefficients in balancer.SolutionSets)
                    Assert.True(balancer.ValidateSolution(coefficients));
            }

            Assert.True(unique_solution_set_obtained_once);
            Assert.True(two_set_solution_obtained_once);
        }

        [Fact]
        public void ValidatingSolutions_Simple()
        {
            var balancer = new BalancerInverseBased(equation: "H2+O2=H2O");
            Assert.True(balancer.ValidateSolution(new BigInteger[] { -2, -1, 2 }));
            Assert.True(balancer.ValidateSolution(new BigInteger[] { -4, -2, 4 }));
            Assert.False(balancer.ValidateSolution(new BigInteger[] { -10, 7, -3 }));
            Assert.Throws<ArgumentException>(() => _ = balancer.ValidateSolution(new BigInteger[] { -2, -1, 2, 2 }));
        }
    }
}
