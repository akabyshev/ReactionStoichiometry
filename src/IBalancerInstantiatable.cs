namespace ReactionStoichiometry;

internal interface IBalancerInstantiatable : IFragmentStore
{
    Func<Int32, String> LabelFor { get; }
    String GetCoefficientExpression(Int32 index);
    String Instantiate(Int64[] parameters);
}