namespace ReactionStoichiometry;

internal abstract class BalancerRisteskiGeneric<T> : AbstractBalancer<T> where T : struct, IEquatable<T>, IFormattable
{
    private List<Int64[]>? _dependentCoefficientExpressions;
    private Int32[]? _freeCoefficientIndices;

    private protected override String Outcome
    {
        get
        {
            if (_dependentCoefficientExpressions == null) return "<FAIL>";
            if (_freeCoefficientIndices == null) throw new InvalidOperationException("This should never ever happen");

            List<String> lines = new() { GetEquationWithPlaceholders() + ", where" };
            lines.AddRange(_dependentCoefficientExpressions.Select(GetExpressionAsString));
            lines.Add("for any {" + String.Join(", ", _freeCoefficientIndices.Select(LabelFor)) + "}");

            return String.Join(Environment.NewLine, lines);
        }
    }

    protected BalancerRisteskiGeneric(String equation) : base(equation)
    {
    }

    internal String GetCoefficientString(Int32 index) =>
        (_freeCoefficientIndices ?? Array.Empty<Int32>()).Contains(index) ? String.Empty : GetExpressionAsString(_dependentCoefficientExpressions![index]);

    private String GetExpressionAsString(Int64[] expression)
    {
        var pivotIndex = Array.FindIndex(expression, i => i != 0);

        var numeratorParts = Enumerable.Range(pivotIndex + 1, expression.Length - (pivotIndex + 1))
                                       .Where(i => expression[i] != 0)
                                       .Select(i =>
                                               {
                                                   var coefficient = (-1 * expression[i]).ToString();
                                                   if (coefficient == "1") coefficient = String.Empty;
                                                   if (coefficient == "-1") coefficient = "-";
                                                   return $"{coefficient}{LabelFor(i)}";
                                               })
                                       .ToList();

        var result = String.Join(" + ", numeratorParts).Replace("+ -", "- ");

        if (expression[pivotIndex] != 1)
        {
            if (numeratorParts.Count > 1) result = $"({result})";
            result = $"{result}/{expression[pivotIndex]}";
        }

        if (result == String.Empty) result = "0";

        return $"{LabelFor(pivotIndex)} = {result}";
    }

    protected override void Balance()
    {
        for (var c = ReactantsCount; c < Fragments.Count; c++)
        {
            M.SetColumn(c, M.Column(c).Multiply(-1));
        }

        var reducedAugmentedMatrix = GetReducedAugmentedMatrix();
        Details.AddRange(Utils.PrettyPrintMatrix("RREF-data augmented matrix", reducedAugmentedMatrix.ToArray(), PrettyPrinter));

        _freeCoefficientIndices = Enumerable.Range(0, reducedAugmentedMatrix.ColumnCount)
                                            .Where(c => reducedAugmentedMatrix.CountNonZeroesInColumn(c) > 1)
                                            .ToArray();
        if (!_freeCoefficientIndices.Any()) throw new BalancerException("This SLE is unsolvable");

        _dependentCoefficientExpressions = Enumerable.Range(0, reducedAugmentedMatrix.RowCount)
                                                     .Select(r => ScaleToIntegers(reducedAugmentedMatrix.GetRow(r)))
                                                     .ToList();
    }

    protected abstract AbstractReducibleMatrix<T> GetReducedAugmentedMatrix();

    internal String Instantiate(Int64[] parameters)
    {
        if (_freeCoefficientIndices == null || _dependentCoefficientExpressions == null)
            throw new InvalidOperationException("This call should have never happened");

        if (parameters.Length != _freeCoefficientIndices.Length) throw new ArgumentOutOfRangeException(nameof(parameters), "Parameters array size mismatch");

        var coefficients = new Int64[Fragments.Count];

        for (var i = 0; i < _freeCoefficientIndices.Length; i++)
        {
            coefficients[_freeCoefficientIndices[i]] = parameters[i];
        }

        for (var j = 0; j < _dependentCoefficientExpressions.Count; j++)
        {
            var expression = _dependentCoefficientExpressions[j];
            var calculated = _freeCoefficientIndices.Sum(t => coefficients[t] * expression[t]);

            if (calculated % expression[j] != 0) throw new InvalidOperationException("Non-integer coefficient, try other SLE params");

            calculated /= expression[j];

            if (j >= ReactantsCount) calculated *= -1;

            coefficients[j] = calculated;
        }

        return GetEquationWithCoefficients(coefficients);
    }
}