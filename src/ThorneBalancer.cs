using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class ThorneBalancer : Balancer<double>
{
    public ThorneBalancer(string equation) : base(equation)
    {
    }

    protected override void Balance()
    {
        int nullity = matrix.ColumnCount - matrix.Rank();

        Matrix<double> GetAugmentedReducedMatrix()
        {
            var reduced = matrix.Clone();
            if ((matrix.RowCount == matrix.ColumnCount))
            {
                var temp = new DoubleMatrixInRREF(matrix);
                reduced = temp.ToMatrix();
            }

            if (reduced.RowCount == reduced.ColumnCount)
            {
                throw new BalancerException("Matrix in RREF is still square");
            }

            var zeros = Matrix<double>.Build.Dense(nullity, matrix.Rank());
            var identity = Matrix<double>.Build.DenseIdentity(nullity);
            var result = reduced.Stack(zeros.Append(identity));

            result.CoerceZero(Helpers.FP_TOLERANCE);

            if (result.HasZeroDeterminant())
            {
                diagnostics.AddRange(Helpers.PrettyPrintMatrix("Zero-determinant matrix", result.ToArray(), PrettyPrinter));
                throw new BalancerException("Matrix can't be inverted");
            }

            return result;
        }

        Matrix<double> augmented_matrix = GetAugmentedReducedMatrix();
        Matrix<double> inverted_augmented_matrix = augmented_matrix.Inverse();
        details.AddRange(Helpers.PrettyPrintMatrix("Inverse of the augmented matrix", inverted_augmented_matrix.ToArray(), PrettyPrinter));

        List<string> independent_equations = new();
        foreach (int i in Enumerable.Range(inverted_augmented_matrix.ColumnCount - nullity, nullity))
        {
            var scaled_nsv = VectorScaler(inverted_augmented_matrix.Column(i).ToArray());
            independent_equations.Add(GetEquationWithCoefficients(scaled_nsv));

            diagnostics.Add(string.Join('\t', scaled_nsv));
        }

        Outcome = string.Join(Environment.NewLine, independent_equations);
    }

    protected override long[] VectorScaler(double[] v)
    {
        return Helpers.ScaleDoubles(v);
    }

    private string GetEquationWithCoefficients(in long[] coefs)
    {
        List<string> l = new();
        List<string> r = new();

        for (int i = 0; i < fragments.Count; i++)
        {
            if (coefs[i] == 0)
                continue;

            var coef_abs = Math.Abs(coefs[i]);
            var t = ((coef_abs == 1) ? "" : coef_abs.ToString() + MULTIPLICATION_SYMBOL) + fragments[i];
            (coefs[i] < 0 ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }

    protected override string PrettyPrinter(double value)
    {
        return Helpers.PrettyPrintDouble(value);
    }
}