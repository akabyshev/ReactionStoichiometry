namespace ReactionStoichiometry;

using System.Numerics;

internal interface ISubstancesList
{
    internal Int32 SubstancesCount { get; }
    internal String GetSubstance(Int32 i);
}

internal interface IBalancerInstantiatable : ISubstancesList
{
    internal String LabelFor(Int32 i);
    internal String? GetCoefficientExpressionString(Int32 index);
    internal (BigInteger[] coefficients, String readable) Instantiate(BigInteger[] parameters);
}