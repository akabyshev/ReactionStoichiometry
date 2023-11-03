namespace ReactionStoichiometry;

using System.Numerics;

internal interface IBalancerInstantiatable : IChemicalEntityList
{
    String LabelFor(Int32 i);
    String? GetCoefficientExpressionString(Int32 index);
    (BigInteger[] coefficients, String readable) Instantiate(BigInteger[] parameters);
}