namespace ReactionStoichiometry;

internal struct Basics_<T>
{
    internal Func<T, T, T> Add;
    internal Func<T, T, T> Subtract;
    internal Func<T, T, T> Multiply;
    internal Func<T, T, T> Divide;
    internal Func<T, bool> IsNonZero;
    internal Func<T, string> AsString;
};