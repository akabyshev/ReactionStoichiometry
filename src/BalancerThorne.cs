namespace ReactionStoichiometry;

using Extensions;
using MathNet.Numerics.LinearAlgebra;

internal class BalancerThorne : AbstractBalancer<Double>
{
    private List<Int64[]>? _independentEquations;

    private protected override String Outcome
    {
        get
        {
            if (_independentEquations == null) return "<FAIL>";

            return String.Join(Environment.NewLine, _independentEquations.Select(c => GetEquationWithCoefficients(c)));
        }
    }

    public BalancerThorne(String equation) : base(equation)
    {
    }

    protected override void Balance()
    {
        var originalMatrixNullity = M.Nullity();
        var invertedAugmentedMatrix = GetAugmentedMatrix().Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", invertedAugmentedMatrix.ToArray(), PrettyPrinter));

        _independentEquations = Enumerable.Range(invertedAugmentedMatrix.ColumnCount - originalMatrixNullity, originalMatrixNullity)
                                          .Select(c => ScaleToIntegers(invertedAugmentedMatrix.Column(c).ToArray()))
                                          .ToList();
    }

    private Matrix<Double> GetAugmentedMatrix()
    {
        var reduced = M.RowCount == M.ColumnCount ? ReducedMatrixOfDouble.CreateInstance(M).ToMatrix() : M.Clone();

        if (reduced.RowCount == reduced.ColumnCount) throw new BalancerException("Matrix in RREF is still square");

        var nullity = reduced.Nullity();
        var submatrixLeftZeroes = Matrix<Double>.Build.Dense(nullity, reduced.ColumnCount - nullity);
        var submatrixRightIdentity = Matrix<Double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(submatrixLeftZeroes.Append(submatrixRightIdentity));

        result.CoerceZero(Program.DOUBLE_PSEUDOZERO);

        if (DoubleExtensions.IsNonZero(result.Determinant())) return result;

        Diagnostics.AddRange(Utils.PrettyPrintMatrix("Zero-determinant matrix", result.ToArray(), PrettyPrinter));
        throw new BalancerException("Matrix can't be inverted");
    }

    protected override Int64[] ScaleToIntegers(Double[] v) => Utils.ScaleDoubles(v);

    private String GetEquationWithCoefficients(in Int64[] coefficients)
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < Fragments.Count; i++)
        {
            if (coefficients[i] == 0) continue;

            var value = Math.Abs(coefficients[i]);
            var t = (value == 1 ? "" : value + Program.MULTIPLICATION_SYMBOL) + Fragments[i];
            (coefficients[i] < 0 ? l : r).Add(t);
        }

        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    protected override String PrettyPrinter(Double value) => Utils.PrettyPrintDouble(value);
}