namespace ReactionStoichiometry;

internal abstract class BalancerRisteskiGeneric<T> : AbstractBalancer<T> where T : struct, IEquatable<T>, IFormattable
{
    private List<Int64[]>? _dependentCoefficientExpressions;
    private List<Int32>? _freeCoefficientIndices;

    private protected override String Outcome
    {
        get
        {
            if (_dependentCoefficientExpressions == null) return "<FAIL>";
            if (_freeCoefficientIndices == null) throw new InvalidOperationException("This should never ever happen");

            List<String> lines = new() { GetEquationWithPlaceholders() + ", where" };
            foreach (var expression in _dependentCoefficientExpressions)
            {
                var pivotIndex = Array.FindIndex(expression, i => i != 0);

                var numeratorParts = Enumerable.Range(pivotIndex + 1, expression.Length - (pivotIndex + 1))
                                               .Where(i => expression[i] != 0)
                                               .Select(c =>
                                                       {
                                                           var coefficient = (-1 * expression[c]).ToString();
                                                           if (coefficient == "1") coefficient = String.Empty;
                                                           if (coefficient == "-1") coefficient = "-";
                                                           return $"{coefficient}{LabelFor(c)}";
                                                       })
                                               .ToList();

                var readableExpression = String.Join(" + ", numeratorParts).Replace("+ -", "- ");

                if (expression[pivotIndex] != 1)
                {
                    if (numeratorParts.Count > 1) readableExpression = $"({readableExpression})";
                    readableExpression = $"{readableExpression}/{expression[pivotIndex]}";
                }

                if (readableExpression == String.Empty) readableExpression = "0";

                lines.Add($"{LabelFor(pivotIndex)} = {readableExpression}");
            }

            lines.Add("for any {" + String.Join(", ", _freeCoefficientIndices.Select(LabelFor)) + "}");

            return String.Join(Environment.NewLine, lines);
        }
    }

    protected BalancerRisteskiGeneric(String equation) : base(equation)
    {
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
                                            .ToList();
        if (!_freeCoefficientIndices.Any()) throw new BalancerException("This SLE is unsolvable");

        _dependentCoefficientExpressions = Enumerable.Range(0, reducedAugmentedMatrix.RowCount)
                                                     .Select(r => ScaleToIntegers(reducedAugmentedMatrix.GetRow(r)))
                                                     .ToList();
    }

    protected abstract AbstractReducibleMatrix<T> GetReducedAugmentedMatrix();
}