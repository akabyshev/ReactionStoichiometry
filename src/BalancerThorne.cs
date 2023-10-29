namespace ReactionStoichiometry;

internal class BalancerThorne : AbstractBalancer<Double>
{
    public BalancerThorne(String equation) : base(equation)
    {
    }

    protected override void BalanceImplementation()
    {
        var originalMatrixNullity = M.ColumnCount - M.Rank();
        var invertedAugmentedMatrix = GetAugmentedMatrix().Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", invertedAugmentedMatrix.ToArray(), PrettyPrinter));

        var independentEquations = new List<String>();
        for (var c = invertedAugmentedMatrix.ColumnCount - originalMatrixNullity; c < invertedAugmentedMatrix.ColumnCount; c++)
        {
            var coefficients = ScaleToIntegers(invertedAugmentedMatrix.Column(c).ToArray());
            independentEquations.Add(GetEquationWithCoefficients(coefficients));
        }

        Outcome = String.Join(Environment.NewLine, independentEquations);
    }

    private MathNet.Numerics.LinearAlgebra.Matrix<Double> GetAugmentedMatrix()
    {
        var reduced = M.RowCount == M.ColumnCount ? ReducedMatrixOfDouble.CreateInstance(M).ToMatrix() : M.Clone();

        if (reduced.RowCount == reduced.ColumnCount) throw new ApplicationSpecificException("Matrix in RREF is still square");

        var nullity = reduced.ColumnCount - reduced.Rank();
        var submatrixLeftZeroes = MathNet.Numerics.LinearAlgebra.Matrix<Double>.Build.Dense(nullity, reduced.ColumnCount - nullity);
        var submatrixRightIdentity = MathNet.Numerics.LinearAlgebra.Matrix<Double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(submatrixLeftZeroes.Append(submatrixRightIdentity));

        result.CoerceZero(Program.DOUBLE_PSEUDOZERO);

        if (ReactionStoichiometry.Extensions.DoubleExtensions.IsNonZero(result.Determinant())) return result;

        Diagnostics.AddRange(Utils.PrettyPrintMatrix("Zero-determinant matrix", result.ToArray(), PrettyPrinter));
        throw new ApplicationSpecificException("Matrix can't be inverted");
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