namespace ReactionStoichiometry;

internal abstract partial class Balancer<T>
{
    internal class BalancerException : InvalidOperationException
    {
        public BalancerException(String message) : base(message)
        {
        }
    }
}