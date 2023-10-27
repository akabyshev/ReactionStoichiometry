namespace ReactionStoichiometry;

internal abstract class AbstractBalancerRisteski<T> : AbstractBalancer<T> where T : struct, IEquatable<T>, IFormattable
{
    protected AbstractBalancerRisteski(String equation) : base(equation)
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

        var freeVarIndices = Enumerable.Range(0, reducedAugmentedMatrix.ColumnCount).Where(c => reducedAugmentedMatrix.CountNonZeroesInColumn(c) > 1).ToList();
        if (!freeVarIndices.Any()) throw new ApplicationSpecificException("This SLE is unsolvable");

        var generalizedEquation = new List<String> { GetEquationWithPlaceholders() + ", where" };
        for (var r = 0; r < reducedAugmentedMatrix.RowCount; r++)
        {
            var coefficients = ScaleToIntegers(reducedAugmentedMatrix.GetRow(r));
            var pivotColumnIndex = Array.FindIndex(coefficients, i => i != 0);

            var parts = Enumerable.Range(pivotColumnIndex + 1, coefficients.Length - (pivotColumnIndex + 1)).Where(c => coefficients[c] != 0).Select(c =>
                    {
                        var coefficient = (-1 * coefficients[c]).ToString();
                        if (coefficient == "1") coefficient = String.Empty;
                        if (coefficient == "-1") coefficient = "-";
                        return $"{coefficient}{LabelFor(c)}";
                    }).ToList();

            var expression = String.Join(" + ", parts).Replace("+ -", "- ");

            if (coefficients[pivotColumnIndex] != 1)
            {
                if (parts.Count > 1) expression = $"({expression})";
                expression = $"{expression}/{coefficients[pivotColumnIndex]}";
            }

            if (expression == String.Empty) expression = "0";

            generalizedEquation.Add($"{LabelFor(pivotColumnIndex)} = {expression}");
        }

        generalizedEquation.Add("for any {" + String.Join(", ", freeVarIndices.Select(LabelFor)) + "}");

        Outcome = String.Join(Environment.NewLine, generalizedEquation);
    }

    protected abstract AbstractReducibleMatrix<T> GetReducedAugmentedMatrix();
}