namespace ReactionStoichiometry;

internal class ApplicationSpecificException : InvalidOperationException
{
    public ApplicationSpecificException(String message) : base(message)
    {
    }
}