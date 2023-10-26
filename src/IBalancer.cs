namespace ReactionStoichiometry;

internal interface IBalancer
{
    string Skeletal { get; }
    string Details { get; }
    string Outcome { get; }
    string Diagnostics { get; }
}