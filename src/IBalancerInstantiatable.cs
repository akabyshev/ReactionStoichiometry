﻿namespace ReactionStoichiometry;

using System.Numerics;

internal interface IBalancerInstantiatable : IChemicalEntityList
{
    String LabelFor(Int32 i);
    String? GetCoefficientExpressionString(Int32 index);
    String Instantiate(BigInteger[] parameters);
}