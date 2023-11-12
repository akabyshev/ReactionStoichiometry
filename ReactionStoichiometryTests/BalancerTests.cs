using System.Numerics;
using ReactionStoichiometry;


namespace ReactionStoichiometryTests
{
    public sealed class BalancerTests
    {
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
    }
}
