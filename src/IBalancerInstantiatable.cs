namespace ReactionStoichiometry;

using System.Numerics;

internal interface IBalancerInstantiatable : IChemicalEntityList
{
    String LabelFor(Int32 i);
    String GetCoefficientExpression(Int32 index);
    String Instantiate(BigInteger[] parameters);
}