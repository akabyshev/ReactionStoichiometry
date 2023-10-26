namespace ReactionStoichiometry;

internal abstract class AbstractBalancerRisteski<T> : AbstractBalancer<T> where T : struct, IEquatable<T>, IFormattable
{
    protected AbstractBalancerRisteski(string equation) : base(equation)
    {
    }

    protected override void Balance()
    {
        for (var c = ReactantsCount; c < fragments.Count; c++)
        {
            matrix.SetColumn(c, matrix.Column(c).Multiply(-1));
        }

        var ram = GetReducedAugmentedMatrix();
        details.AddRange(Helpers.PrettyPrintMatrix("RREF-data augmented matrix", ram.ToArray(), PrettyPrinter));

        var freeVarIndices =
            Enumerable.Range(0, ram.ColumnCount).Where(c => ram.CountNonZeroesInColumn(c) > 1).ToList();
        if (!freeVarIndices.Any()) throw new ApplicationSpecificException("This SLE is unsolvable");

        List<string> expressions = new();
        for (var r = 0; r < ram.RowCount; r++)
        {
            var coefficients = ScaleToIntegers(ram.GetRow(r));
            var pivotColumnIndex = Array.FindIndex(coefficients, i => i != 0);

            var parts = new List<string>();
            for (var c = pivotColumnIndex + 1; c < coefficients.Length; c++)
            {
                if (coefficients[c] == 0) continue;

                var coefficient = (-1 * coefficients[c]).ToString();
                if (coefficient == "1") coefficient = string.Empty;
                if (coefficient == "-1") coefficient = "-";

                parts.Add($"{coefficient}{LabelFor(c)}");
            }

            var expression = string.Join(" + ", parts).Replace("+ -", "- ");

            if (coefficients[pivotColumnIndex] != 1)
            {
                if (parts.Count > 1) expression = $"({expression})";
                expression = $"{expression}/{coefficients[pivotColumnIndex]}";
            }

            if (expression == string.Empty) expression = "0";

            expressions.Add($"{LabelFor(pivotColumnIndex)} = {expression}");
        }

        List<string> generalizedSolution = new() { GetEquationWithPlaceholders() + ", where" };
        generalizedSolution.AddRange(expressions);
        generalizedSolution.Add("for any {" + string.Join(", ", freeVarIndices.Select(LabelFor)) + "}");

        Outcome = string.Join(Environment.NewLine, generalizedSolution);
    }

    protected abstract AbstractReducibleMatrix<T> GetReducedAugmentedMatrix();
}