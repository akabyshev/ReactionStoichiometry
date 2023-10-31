namespace ReactionStoichiometry;

using System.Numerics;

internal abstract class BalancerRisteski<T> : Balancer<T>, IBalancerInstantiatable where T : struct, IEquatable<T>, IFormattable
{
    private readonly Dictionary<Int32, BigInteger[]> _dependentCoefficientExpressions = new();
    private List<Int32> _freeCoefficientIndices = null!;

    protected override IEnumerable<String> Outcome
    {
        get
        {
            if (_dependentCoefficientExpressions.Count == 0) return new[] { Program.FAILED_BALANCING_OUTCOME };

            List<String> lines = new() { GetEquationWithPlaceholders + ", where" };
            lines.AddRange(_dependentCoefficientExpressions.Keys.Select(i => $"{LabelFor(i)} = {GetCoefficientExpression(i)}"));
            lines.Add("for any {" + String.Join(", ", _freeCoefficientIndices.Select(LabelFor)) + "}");

            return lines;
        }
    }

    protected BalancerRisteski(String equation) : base(equation)
    {
    }

    public String GetCoefficientExpression(Int32 index)
    {
        if (!_dependentCoefficientExpressions.ContainsKey(index)) return String.Empty;

        var expression = _dependentCoefficientExpressions[index];

        var numeratorParts = Enumerable.Range(index + 1, expression.Length - (index + 1))
                                       .Where(i => expression[i] != 0)
                                       .Select(i =>
                                               {
                                                   var coefficient = (-1 * expression[i]).ToString();
                                                   if (coefficient == "1") coefficient = String.Empty;
                                                   if (coefficient == "-1") coefficient = "-";
                                                   return $"{coefficient}{LabelFor(i)}"; // TODO: Streamline
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

    public String Instantiate(BigInteger[] parameters)
    {
        if (parameters.Length != _freeCoefficientIndices.Count) throw new ArgumentOutOfRangeException(nameof(parameters), "Parameters array size mismatch");

        var coefficients = new BigInteger[Entities.Count];

        for (var i = 0; i < _freeCoefficientIndices.Count; i++)
        {
            coefficients[_freeCoefficientIndices[i]] = parameters[i];
        }

        foreach (var kvp in _dependentCoefficientExpressions)
        {
            var calculated = _freeCoefficientIndices.Aggregate(BigInteger.Zero, (sum, i) => sum + coefficients[i] * kvp.Value[i]);

            if (calculated % kvp.Value[kvp.Key] != 0) throw new BalancerException("Non-integer coefficient, try other SLE params");

            calculated /= kvp.Value[kvp.Key];

            if (kvp.Key >= ReactantsCount) calculated *= -1;

            coefficients[kvp.Key] = calculated;
        }

        return GetEquationWithCoefficients(coefficients);
    }

    protected override void Balance()
    {
        M.MapIndexedInplace((_, c, value) => c >= ReactantsCount ? -value : value);

        var reducedAugmentedMatrix = GetReducedAugmentedMatrix();
        Details.AddRange(Utils.PrettyPrintMatrix("RREF-data augmented matrix", reducedAugmentedMatrix.ToArray(), PrettyPrinter));

        _freeCoefficientIndices = Enumerable.Range(0, reducedAugmentedMatrix.ColumnCount)
                                            .Where(c => reducedAugmentedMatrix.CountNonZeroesInColumn(c) > 1)
                                            .ToList();
        if (_freeCoefficientIndices.Count == 0) throw new BalancerException("This SLE is unsolvable");

        for (var r = 0; r < reducedAugmentedMatrix.RowCount; r++)
        {
            var row = ScaleToIntegers(reducedAugmentedMatrix.GetRow(r));
            var dependentCoefficientIndex = Array.FindIndex(row, static i => i != 0);
            _dependentCoefficientExpressions.Add(dependentCoefficientIndex, row);
        }
    }

    protected abstract SpecialMatrixReducible<T> GetReducedAugmentedMatrix();
}