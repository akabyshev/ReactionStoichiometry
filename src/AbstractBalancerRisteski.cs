namespace ReactionStoichiometry;

internal abstract class AbstractBalancerRisteski<T> : AbstractBalancer<T> where T : struct, IEquatable<T>, IFormattable
{
    public AbstractBalancerRisteski(string equation) : base(equation)
    {
    }

    protected override void Balance()
    {
        for (var i_c = ReactantsCount; i_c < fragments.Count; i_c++)
        {
            matrix.SetColumn(i_c, matrix.Column(i_c).Multiply(-1));
        }

        var RAM = GetReducedAugmentedMatrix();
        details.AddRange(Helpers.PrettyPrintMatrix("RREF-data augmented matrix", RAM.ToArray(), PrettyPrinter));

        var free_var_indices = Enumerable.Range(0, RAM.ColumnCount).Where(i_c => RAM.CountNonZeroesInColumn(i_c) > 1);
        if (!free_var_indices.Any())
        {
            throw new ApplicationSpecificException("This SLE is unsolvable");
        }

        List<string> expressions = new();
        for (var i_r = 0; i_r < RAM.RowCount; i_r++)
        {
            var scaled_row = VectorScaler(RAM.GetRow(i_r));
            var pivot_column = Array.FindIndex(scaled_row, i => i != 0);

            var expression_parts = new List<string>();
            for (var i_c = pivot_column + 1; i_c < scaled_row.Length; i_c++)
            {
                if (scaled_row[i_c] != 0)
                {
                    var coef = (-1 * scaled_row[i_c]).ToString();
                    if (coef == "1")
                    {
                        coef = string.Empty;
                    }
                    if (coef == "-1")
                    {
                        coef = "-";
                    }

                    expression_parts.Add($"{coef}{Labeller(i_c)}");
                }
            }
            var expression = string.Join(" + ", expression_parts).Replace("+ -", "- ");

            if (scaled_row[pivot_column] != 1)
            {
                if (expression_parts.Count > 1)
                {
                    expression = $"({expression})";
                }
                expression = $"{expression}/{scaled_row[pivot_column]}";
            }

            if (expression == string.Empty)
            {
                expression = "0";
            }

            expressions.Add($"{Labeller(pivot_column)} = {expression}");
        }

        List<string> generalized_solution = new();
        generalized_solution.Add(GetEquationWithPlaceholders() + ", where");
        generalized_solution.AddRange(expressions);
        generalized_solution.Add("for any {" + string.Join(", ", free_var_indices.Select(Labeller)) + "}");

        Outcome = string.Join(Environment.NewLine, generalized_solution);
    }

    protected abstract AbstractReducibleMatrix<T> GetReducedAugmentedMatrix();

    private string GetEquationWithPlaceholders()
    {
        List<string> l = new();
        List<string> r = new();

        for (var i = 0; i < fragments.Count; i++)
        {
            var t = Labeller(i) + MULTIPLICATION_SYMBOL + fragments[i];
            (i < ReactantsCount ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }
}