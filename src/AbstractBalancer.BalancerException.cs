namespace ReactionStoichiometry;

internal abstract partial class AbstractBalancer<T>
{
    internal class BalancerException : InvalidOperationException
    {
        public BalancerException(String message) : base(message)
        {
        }
    }
}