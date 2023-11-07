namespace ReactionStoichiometry;

using Rationals;

internal static class PredefinedBasicOperations
{
    internal static SpecialMatrix<Double>.BasicOperations BasicOperationsOfDouble =>
        new()
        {
            Add = static (d1, d2) => d1 + d2,
            Subtract = static (d1, d2) => d1 - d2,
            Multiply = static (d1, d2) => d1 * d2,
            Divide = static (d1, d2) => d1 / d2,
            IsZero = static d => Utils.IsZeroDouble(d),
            IsOne = static d => Utils.IsZeroDouble(1.0d - d),
            AsString = static d => d.ToString()
        };

    internal static SpecialMatrix<Rational>.BasicOperations BasicOperationsOfRational =>
        new()
        {
            Add = Rational.Add,
            Subtract = Rational.Subtract,
            Multiply = Rational.Multiply,
            Divide = Rational.Divide,
            IsZero = static r => r.IsZero,
            IsOne = static r => r.IsOne,
            AsString = static r => r.ToString("C")
        };
}
