namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal sealed class BalancerThorne : Balancer<Double>
{
    private List<BigInteger[]>? _independentEquations;

    protected override IEnumerable<String> Outcome
    {
        get
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_independentEquations == null) return new[] { Program.FAILED_BALANCING_OUTCOME };

            return _independentEquations.Select(EquationWithIntegerCoefficients);
        }
    }

    public BalancerThorne(String equation) : base(equation, Utils.PrettyPrintDouble, Utils.ScaleDoubles)
    {
    }

    protected override void Balance()
    {
        var augmentedMatrix = GetInvertibleAugmentedMatrix();
        if (!Utils.IsNonZeroDouble(augmentedMatrix.Determinant())) throw new BalancerException("Augmented matrix can't be inverted");
        var invertedAugmentedMatrix = augmentedMatrix.Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", invertedAugmentedMatrix.ToArray(), PrettyPrinter));


        _independentEquations = Enumerable.Range(invertedAugmentedMatrix.ColumnCount - M.Nullity(), M.Nullity())
                                          .Select(c => ScaleToIntegers(invertedAugmentedMatrix.Column(c).ToArray()))
                                          .ToList();
    }

    private Matrix<Double> GetInvertibleAugmentedMatrix()
    {
        var reduced = M.RowCount == M.ColumnCount ? SpecialMatrixReducedDouble.CreateInstance(M).ToMatrix() : M.Clone();

        if (reduced.RowCount == reduced.ColumnCount) throw new BalancerException("Matrix in RREF is still square");

        var nullity = reduced.Nullity();
        var submatrixLeftZeroes = Matrix<Double>.Build.Dense(nullity, reduced.ColumnCount - nullity);
        var submatrixRightIdentity = Matrix<Double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(submatrixLeftZeroes.Append(submatrixRightIdentity));

        result.CoerceZero(Program.GOOD_ENOUGH_DOUBLE_ZERO);

        return result;
    }
}