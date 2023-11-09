namespace ReactionStoichiometry;

internal sealed class BalancerException : InvalidOperationException
{
    private BalancerException(String message) : base(message)
    {
    }

    internal static void ThrowIf(Boolean condition, String message)
    {
        if (condition)
            throw new BalancerException(message);
    }
}
