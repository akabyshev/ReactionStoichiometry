using MathNet.Numerics.LinearAlgebra;
using ReactionStoichiometry.Extensions;

namespace ReactionStoichiometry;

internal class BalancerThorne : AbstractBalancer<double>
{
    public BalancerThorne(string equation) : base(equation)
    {
    }

    protected override void Balance()
    {
        var nullity = matrix.ColumnCount - matrix.Rank();

        var augmentedMatrix = GetAugmentedMatrix(nullity);
        var invertedAugmentedMatrix = augmentedMatrix.Inverse();
        details.AddRange(Helpers.PrettyPrintMatrix("Inverse of the augmented matrix", invertedAugmentedMatrix.ToArray(),
            PrettyPrinter));

        List<string> independentEquations = new();
        foreach (var i in Enumerable.Range(invertedAugmentedMatrix.ColumnCount - nullity, nullity))
        {
            var coefficients = ScaleToIntegers(invertedAugmentedMatrix.Column(i).ToArray());
            independentEquations.Add(GetEquationWithCoefficients(coefficients));

            diagnostics.Add(string.Join(Environment.NewLine, coefficients));
        }

        Outcome = string.Join(Environment.NewLine, independentEquations);
    }

    private Matrix<double> GetAugmentedMatrix(int nullity)
    {
        var reduced = matrix.RowCount == matrix.ColumnCount
            ? new ReducedMatrixOfDouble(matrix).ToMatrix()
            : matrix.Clone();

        if (reduced.RowCount == reduced.ColumnCount)
            throw new ApplicationSpecificException("Matrix in RREF is still square");

        var zeros = Matrix<double>.Build.Dense(nullity, matrix.Rank());
        var identity = Matrix<double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(zeros.Append(identity));

        result.CoerceZero(Program.DOUBLE_PSEUDOZERO);

        if (result.Determinant().IsNonZero())
            return result;

        diagnostics.AddRange(Helpers.PrettyPrintMatrix("Zero-determinant matrix", result.ToArray(), PrettyPrinter));
        throw new ApplicationSpecificException("Matrix can't be inverted");
    }

    protected override long[] ScaleToIntegers(double[] v)
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
            var t = (value == 1 ? "" : value + Program.MULTIPLICATION_SYMBOL) + fragments[i];
            (coefficients[i] < 0 ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }

    protected override string PrettyPrinter(double value)
    {
        return Helpers.PrettyPrintDouble(value);
    }
}