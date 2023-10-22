namespace ReactionStoichiometry;

internal class BalancerException : InvalidOperationException
{
    public BalancerException(string message) : base(message) { }
}