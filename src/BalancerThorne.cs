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
        var originalMatrixNullity = M.ColumnCount - M.Rank();
        var invertedAugmentedMatrix = GetAugmentedMatrix().Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", invertedAugmentedMatrix.ToArray(), PrettyPrinter));

        var independentEquations = new List<string>();
        for (var c = invertedAugmentedMatrix.ColumnCount - originalMatrixNullity; c < invertedAugmentedMatrix.ColumnCount; c++)
        {
            var coefficients = ScaleToIntegers(invertedAugmentedMatrix.Column(c).ToArray());
            independentEquations.Add(GetEquationWithCoefficients(coefficients));
        }

        Outcome = string.Join(Environment.NewLine, independentEquations);
    }

    private Matrix<double> GetAugmentedMatrix()
    {
        var reduced = M.RowCount == M.ColumnCount
            ? ReducedMatrixOfDouble.CreateInstance(M).ToMatrix()
            : M.Clone();

        if (reduced.RowCount == reduced.ColumnCount)
            throw new ApplicationSpecificException("Matrix in RREF is still square");

        var nullity = reduced.ColumnCount - reduced.Rank();
        var submatrixLeftZeroes = Matrix<double>.Build.Dense(nullity, reduced.ColumnCount - nullity);
        var submatrixRightIdentity = Matrix<double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(submatrixLeftZeroes.Append(submatrixRightIdentity));

        result.CoerceZero(Program.DOUBLE_PSEUDOZERO);

        if (result.Determinant().IsNonZero())
            return result;

        Diagnostics.AddRange(Utils.PrettyPrintMatrix("Zero-determinant matrix", result.ToArray(), PrettyPrinter));
        throw new ApplicationSpecificException("Matrix can't be inverted");
    }

    protected override long[] ScaleToIntegers(double[] v)
    {
        return Utils.ScaleDoubles(v);
    }

    private string GetEquationWithCoefficients(in long[] coefficients)
    {
        List<string> l = new();
        List<string> r = new();

        for (var i = 0; i < Fragments.Count; i++)
        {
            if (coefficients[i] == 0)
                continue;

            var value = Math.Abs(coefficients[i]);
            var t = (value == 1 ? "" : value + Program.MULTIPLICATION_SYMBOL) + Fragments[i];
            (coefficients[i] < 0 ? l : r).Add(t);
        }

        return string.Join(" + ", l) + " = " + string.Join(" + ", r);
    }

    protected override string PrettyPrinter(double value)
    {
        return Utils.PrettyPrintDouble(value);
    }
}