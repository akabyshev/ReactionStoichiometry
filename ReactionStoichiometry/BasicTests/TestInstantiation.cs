namespace ReactionStoichiometry;

using System.Numerics;

internal abstract class TestInstantiation : BasicTest
{
    internal static void Run()
    {
        using StreamReader reader = new(ConstructPath(nameof(TestInstantiation)));
        while (reader.ReadLine() is { } line)
        {
            if (line.StartsWith(IGNORED_LINE_MARK) || line.Length == 0)
                continue;
            var parts = line.Split(CHAR_TAB);
            var eq = parts[0].Replace(oldValue: " ", String.Empty);

            var generalized = new BalancerGeneralized(eq);
            var inverseBased = new BalancerInverseBased(eq);

            generalized.Run();
            inverseBased.Run();

            if (String.IsNullOrEmpty(parts[1]))
                continue;

            var instances = parts[1]
                            .Split(separator: ';')
                            .Select(selector: static s => s.Trim('(', ')').Split(separator: ',').Select(BigInteger.Parse))
                            .Select(selector: parametersSet =>
                                                  generalized.EquationWithIntegerCoefficients(generalized.Instantiate(parametersSet.ToArray())));

            AssertStringsAreEqual(inverseBased.ToString(Balancer.OutputFormat.OutcomeOnlyCommas), String.Join(separator: ",", instances));
        }
    }
}
