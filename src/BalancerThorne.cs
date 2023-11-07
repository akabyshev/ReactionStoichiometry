namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal sealed class BalancerThorne : Balancer
{
    private List<BigInteger[]>? _independentReactions;

    public BalancerThorne(String equation) : base(equation)
    {
    }

    internal Int32 NumberOfIndependentReactions => _independentReactions!.Count;

    protected override IEnumerable<String> Outcome
    {
        get
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_independentReactions == null) return new[] { "<FAIL>" };

            return _independentReactions.Select(EquationWithIntegerCoefficients);
        }
    }

    protected override void BalanceImplementation()
    {
        BalancerException.ThrowIf(Equation.CompositionMatrix.Nullity() == 0, "Zero null-space");

        var augmentedMatrix = Matrix<Double>.Build.DenseOfArray(GetAugmentedSquareMatrix());
        BalancerException.ThrowIf(Utils.IsZeroDouble(augmentedMatrix.Determinant()), "Augmented matrix can't be inverted");

        var inverse = augmentedMatrix.Inverse();
        Details.AddRange(Utils.PrettyPrint("Inverse of the augmented matrix", inverse.ToArray()));

        _independentReactions = Enumerable.Range(inverse.ColumnCount - Equation.CompositionMatrix.Nullity(), Equation.CompositionMatrix.Nullity())
                                          .Select(c => Utils.ScaleDoubles(inverse.Column(c)))
                                          .ToList();
    }

    internal override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.VectorsNotation) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null) return "<FAIL>";

        return NumberOfIndependentReactions + ":" + String.Join(", ", _independentReactions.Select(static v => '{' + String.Join(", ", v) + '}'));
    }

    private Double[,] GetAugmentedSquareMatrix()
    {
        SpecialMatrix<Double> reduced = new SpecialMatrixReducedDouble(Equation.CompositionMatrix);
        BalancerException.ThrowIf(reduced.RowCount >= reduced.ColumnCount, "The method fails on this kind of equations");

        var rowDeficit = reduced.ColumnCount - reduced.RowCount;

        var result = new Double[reduced.RowCount + rowDeficit, reduced.ColumnCount];
        for (var r = 0; r < reduced.RowCount; r++)
            for (var c = 0; c < reduced.ColumnCount; c++)
                result[r, c] = reduced[r, c];

        for (var r = 0; r < rowDeficit; r++) result[reduced.RowCount + r, reduced.RowCount + r] = 1.0d;

        return result;
    }
}
