namespace ReactionStoichiometry;

using System.Numerics;

internal interface IChemicalEntityList
{
    internal Int32 EntitiesCount { get; }
    internal String GetEntity(Int32 i);
}

internal interface IBalancerInstantiatable : IChemicalEntityList
{
    internal String LabelFor(Int32 i);
    internal String? GetCoefficientExpressionString(Int32 index);
    internal (BigInteger[] coefficients, String readable) Instantiate(BigInteger[] parameters);
}