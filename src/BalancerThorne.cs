namespace ReactionStoichiometry;

using System.Numerics;
using Rationals;

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
        BalancerException.ThrowIf(Equation.CompositionMatrix.Nullity == 0, "Zero null-space");

        var squareMatrix = RationalMatrix.CreateInstance(GetAugmentedSquareMatrix(), static v => v);
        BalancerException.ThrowIf(squareMatrix.Determinant().IsZero, "Augmented matrix can't be inverted"); // todo: ?

        var inverse = squareMatrix.Inverse();
        Details.AddRange(inverse.PrettyPrint("Inverse of the augmented matrix"));

        _independentReactions = Enumerable.Range(inverse.ColumnCount - Equation.CompositionMatrix.Nullity, Equation.CompositionMatrix.Nullity)
                                          .Select(c => Utils.ScaleRationals(inverse.GetColumn(c)))
                                          .ToList();
    }

    public override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.VectorsNotation) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null) return "<FAIL>";

        return NumberOfIndependentReactions + ":" + String.Join(", ", _independentReactions.Select(static v => '{' + String.Join(", ", v) + '}'));
    }

    private Rational[,] GetAugmentedSquareMatrix()
    {
        var reduced = RationalMatrix.CreateInstance(Equation.CompositionMatrix.Reduce(), static r => r);
        BalancerException.ThrowIf(reduced.RowCount >= reduced.ColumnCount, "The method fails on this kind of equations");

        var result = new Rational[reduced.ColumnCount, reduced.ColumnCount];
        for (var r = 0; r < reduced.ColumnCount; r++)
            for (var c = 0; c < reduced.ColumnCount; c++)
                result[r, c] = r < reduced.RowCount ? reduced[r, c] : Rational.Zero;

        for (var r = 0; r < reduced.ColumnCount - reduced.RowCount; r++) result[reduced.RowCount + r, reduced.RowCount + r] = Rational.One;

        return result;
    }
}
