namespace ReactionStoichiometry
{
    public sealed class AppSpecificException : InvalidOperationException
    {
        private AppSpecificException(String message) : base(message)
        {
        }

        public static void ThrowIf(Boolean condition, String message)
        {
            if (condition)
            {
                throw new AppSpecificException(message);
            }
        }
    }
}
