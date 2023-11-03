namespace ReactionStoichiometry;

internal sealed class BalancerException : InvalidOperationException
{
    public BalancerException(String message) : base(message)
    {
    }
}