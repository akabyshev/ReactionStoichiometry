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
        var augmentedMatrix = GetAugmentedSquareMatrix();
        BalancerException.ThrowIf(!Utils.IsNonZeroDouble(augmentedMatrix.Determinant()), "Augmented matrix can't be inverted");

        var inverse = augmentedMatrix.Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", inverse.ToArray()));

        _independentEquations = Enumerable.Range(inverse.ColumnCount - Equation.CompositionMatrix.Nullity(), Equation.CompositionMatrix.Nullity())
                                          .Select(c => Utils.ScaleDoubles(inverse.Column(c)))
                                          .ToList();
    }

    private Matrix<Double> GetAugmentedSquareMatrix()
    {
        Matrix<Double>? reduced;
        if (Equation.CompositionMatrix.RowCount >= Equation.CompositionMatrix.ColumnCount)
        {
            reduced = SpecialMatrixReducedDouble.CreateInstance(Equation.CompositionMatrix).ToMatrix();
            BalancerException.ThrowIf(reduced.RowCount >= reduced.ColumnCount, "Can't (yet?) work with this kind of equations");
        }
        else
            reduced = Equation.CompositionMatrix.Clone();

        var rowDeficit = reduced.ColumnCount - reduced.RowCount;
        var augmentingRows = Matrix<Double>.Build.Dense(rowDeficit, reduced.ColumnCount - rowDeficit).Append(Matrix<Double>.Build.DenseIdentity(rowDeficit));

        var result = reduced.Stack(augmentingRows);
        result.CoerceZero(Properties.Settings.Default.GOOD_ENOUGH_FLOAT_PRECISION);
        return result;
    }

    internal override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.OutcomeVectorNotation) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentEquations == null) return "<FAIL>";

        return String.Join(" and ", _independentEquations.Select(static v => '(' + String.Join(", ", v) + ')'));
    }
}