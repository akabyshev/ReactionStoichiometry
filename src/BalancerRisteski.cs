namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Rationals;

internal abstract class BalancerRisteski<T> : Balancer, IBalancerInstantiatable where T : struct, IEquatable<T>, IFormattable
{
    private readonly Func<IEnumerable<T>, BigInteger[]> _scaleToIntegers;
    private Dictionary<Int32, BigInteger[]>? _dependentCoefficientExpressions;
    private List<Int32>? _freeCoefficientIndices;

    protected BalancerRisteski(String equation, Func<IEnumerable<T>, BigInteger[]> scale) : base(equation) => _scaleToIntegers = scale;

    protected override IEnumerable<String> Outcome
    {
        get
        {
            if (_dependentCoefficientExpressions == null || _freeCoefficientIndices == null) return new[] { Program.FAILED_BALANCING_OUTCOME };

            List<String> lines = new() { EquationWithPlaceholders() + ", where" };
            lines.AddRange(_dependentCoefficientExpressions.Keys.Select(i => $"{LabelFor(i)} = {GetCoefficientExpressionString(i)}"));
            lines.Add("for any {" + String.Join(", ", _freeCoefficientIndices.Select(LabelFor)) + "}");

            return lines;
        }
    }

    protected override void BalanceImplementation()
    {
        var reducedMatrix = GetReducedSignedMatrix();
        BalancerException.ThrowIf(reducedMatrix.IsIdentityMatrix, "This SLE is unsolvable");
        Details.AddRange(Utils.PrettyPrintMatrix("Reduced signed matrix", reducedMatrix.ToArray()));

        _dependentCoefficientExpressions = Enumerable.Range(0, reducedMatrix.RowCount)
                                                     .Select(r => _scaleToIntegers(reducedMatrix.GetRow(r)))
                                                     .ToDictionary(static row => Array.FindIndex(row, static i => i != 0), static row => row);

        _freeCoefficientIndices = Enumerable.Range(0, reducedMatrix.ColumnCount)
                                            .Where(c => !_dependentCoefficientExpressions.ContainsKey(c) && reducedMatrix.CountNonZeroesInColumn(c) > 0)
                                            .ToList();
    }

    protected Matrix<Double> GetSignedCompositionMatrix()
    {
        var result = Equation.CompositionMatrix.Clone();
        result.MapIndexedInplace((_, c, value) => c >= Equation.OriginalReactantsCount ? -value : value);
        return result;
    }

    protected abstract SpecialMatrixReducible<T> GetReducedSignedMatrix();

    private String EquationWithPlaceholders() =>
        Equation.AssembleEquationString(Enumerable.Range(0, EntitiesCount).Select(LabelFor).ToArray(),
                                        static _ => true,
                                        static value => value + Program.MULTIPLICATION_SYMBOL,
                                        (index, _) => index < Equation.OriginalReactantsCount);

    #region IBalancerInstantiatable Members
    public String LabelFor(Int32 i) => EntitiesCount > Program.LETTER_LABEL_THRESHOLD ? Utils.GenericLabel(i) : Utils.LetterLabel(i);

    public String? GetCoefficientExpressionString(Int32 index)
    {
        if (_dependentCoefficientExpressions == null) throw new InvalidOperationException();

        if (!_dependentCoefficientExpressions.ContainsKey(index)) return null;

        var expression = _dependentCoefficientExpressions[index];

        var numeratorParts = Enumerable.Range(index + 1, expression.Length - (index + 1))
                                       .Where(i => expression[i] != 0)
                                       .Select(i =>
                                               {
                                                   var coefficient = (-expression[i]).ToString();
                                                   if (coefficient == "1") coefficient = String.Empty;
                                                   if (coefficient == "-1") coefficient = "-";
                                                   return $"{coefficient}{LabelFor(i)}";
                                               })
                                       .ToList();

        var result = String.Join(" + ", numeratorParts).Replace("+ -", "- ");

        if (expression[index] != 1)
        {
            if (numeratorParts.Count > 1) result = $"({result})";
            result = $"{result}/{expression[index]}";
        }

        if (result == String.Empty) result = "0";

        return result;
    }

    public (BigInteger[] coefficients, String readable) Instantiate(BigInteger[] parameters)
    {
        if (parameters.Length != _freeCoefficientIndices!.Count) throw new ArgumentOutOfRangeException(nameof(parameters), "Array size mismatch");

        var result = new BigInteger[EntitiesCount];

        for (var i = 0; i < _freeCoefficientIndices.Count; i++)
        {
            result[_freeCoefficientIndices[i]] = parameters[i];
        }

        foreach (var kvp in _dependentCoefficientExpressions!)
        {
            var calculated = _freeCoefficientIndices.Aggregate(BigInteger.Zero, (sum, i) => sum + result[i] * kvp.Value[i]);
            BalancerException.ThrowIf(calculated % kvp.Value[kvp.Key] != 0, "Non-integer coefficient, try other SLE params");
            calculated /= kvp.Value[kvp.Key];
            if (kvp.Key >= Equation.OriginalReactantsCount) calculated *= -1;
            result[kvp.Key] = calculated;
        }

        return (result, EquationWithIntegerCoefficients(result));
    }
    #endregion
}

internal sealed class BalancerRisteskiDouble : BalancerRisteski<Double>
{
    public BalancerRisteskiDouble(String equation) : base(equation, Utils.ScaleDoubles)
    {
    }

    protected override SpecialMatrixReducedDouble GetReducedSignedMatrix() => SpecialMatrixReducedDouble.CreateInstance(GetSignedCompositionMatrix());
}

internal sealed class BalancerRisteskiRational : BalancerRisteski<Rational>
{
    public BalancerRisteskiRational(String equation) : base(equation, Utils.ScaleRationals)
    {
    }

    protected override SpecialMatrixReducedRational GetReducedSignedMatrix() => SpecialMatrixReducedRational.CreateInstance(GetSignedCompositionMatrix());
}