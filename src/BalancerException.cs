namespace ReactionStoichiometry;

internal sealed class BalancerException : InvalidOperationException
{
    internal BalancerException(String message) : base(message)
    {
    }
}