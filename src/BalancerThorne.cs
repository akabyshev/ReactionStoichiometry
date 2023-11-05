namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal sealed class BalancerThorne : Balancer
{
    private List<BigInteger[]>? _independentEquations;

    public BalancerThorne(String equation) : base(equation)
    {
    }

    protected override IEnumerable<String> Outcome
    {
        get
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_independentEquations == null) return new[] { "<FAIL>" };

            return _independentEquations.Select(EquationWithIntegerCoefficients);
        }
    }

    protected override void BalanceImplementation()
    {
        var augmentedMatrix = GetAugmentedMatrix();
        BalancerException.ThrowIf(!Utils.IsNonZeroDouble(augmentedMatrix.Determinant()), "Augmented matrix can't be inverted");

        var inverse = augmentedMatrix.Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", inverse.ToArray()));

        _independentEquations = Enumerable.Range(inverse.ColumnCount - Equation.CompositionMatrix.Nullity(), Equation.CompositionMatrix.Nullity())
                                          .Select(c => Utils.ScaleDoubles(inverse.Column(c)))
                                          .ToList();
    }

    private Matrix<Double> GetAugmentedMatrix()
    {
        var reduced = Equation.CompositionMatrix.RowCount == Equation.CompositionMatrix.ColumnCount ?
            SpecialMatrixReducedDouble.CreateInstance(Equation.CompositionMatrix).ToMatrix() :
            Equation.CompositionMatrix.Clone();

        BalancerException.ThrowIf(reduced.RowCount == reduced.ColumnCount, "Reduced matrix is still square");

        var nullity = reduced.Nullity();
        var submatrixLeftZeroes = Matrix<Double>.Build.Dense(nullity, reduced.ColumnCount - nullity);
        var submatrixRightIdentity = Matrix<Double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(submatrixLeftZeroes.Append(submatrixRightIdentity));

        result.CoerceZero(Properties.Settings.Default.GOOD_ENOUGH_FLOAT_PRECISION);

        return result;
    }
}