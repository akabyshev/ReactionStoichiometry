namespace ReactionStoichiometry;

internal sealed class AppSpecificException : InvalidOperationException
{
    private AppSpecificException(String message) : base(message)
    {
    }

    internal static void ThrowIf(Boolean condition, String message)
    {
        if (condition)
            throw new AppSpecificException(message);
    }
}
