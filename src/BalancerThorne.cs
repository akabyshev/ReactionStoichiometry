using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class BalancerThorne : AbstractBalancer<double>
{
    public BalancerThorne(string equation) : base(equation)
    {
    }

    protected override void Balance()
    {
        var nullity = matrix.ColumnCount - matrix.Rank();

        var augmented_matrix = GetAugmentedMatrix(nullity);
        var inverted_augmented_matrix = augmented_matrix.Inverse();
        details.AddRange(Helpers.PrettyPrintMatrix("Inverse of the augmented matrix", inverted_augmented_matrix.ToArray(), PrettyPrinter));

        List<string> independent_equations = new();
        foreach (var i in Enumerable.Range(inverted_augmented_matrix.ColumnCount - nullity, nullity))
        {
            var scaled_nsv = ScaleVectorToIntegers(inverted_augmented_matrix.Column(i).ToArray());
            independent_equations.Add(GetEquationWithCoefficients(scaled_nsv));

            diagnostics.Add(string.Join('\t', scaled_nsv));
        }

        Outcome = string.Join(Environment.NewLine, independent_equations);
    }

    private Matrix<double> GetAugmentedMatrix(int nullity)
    {
        var reduced = (matrix.RowCount == matrix.ColumnCount) ? (new DoubleMatrixInRREF(matrix)).ToMatrix() : matrix.Clone();
            
        if (reduced.RowCount == reduced.ColumnCount)
        {
            throw new ApplicationSpecificException("Matrix in RREF is still square");
        }

        var zeros = Matrix<double>.Build.Dense(nullity, matrix.Rank());
        var identity = Matrix<double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(zeros.Append(identity));

        result.CoerceZero(Helpers.FP_TOLERANCE);

        if (!result.Determinant().IsNonZero())
        {
            diagnostics.AddRange(Helpers.PrettyPrintMatrix("Zero-determinant matrix", result.ToArray(), PrettyPrinter));
            throw new ApplicationSpecificException("Matrix can't be inverted");
        }

        return result;
    }

    protected override long[] ScaleVectorToIntegers(double[] v)
    {
        return Helpers.ScaleDoubles(v);
    }

    private string GetEquationWithCoefficients(in long[] coefficients)
    {
        List<string> l = new();
        List<string> r = new();

        for (var i = 0; i < fragments.Count; i++)
        {
            if (coefficients[i] == 0)
                continue;

            var value = Math.Abs(coefficients[i]);
            var t = ((value == 1) ? "" : value.ToString() + MULTIPLICATION_SYMBOL) + fragments[i];
            (coefficients[i] < 0 ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }

    protected override string PrettyPrinter(double value)
    {
        return Helpers.PrettyPrintDouble(value);
    }
}