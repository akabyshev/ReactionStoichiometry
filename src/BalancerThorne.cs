namespace ReactionStoichiometry;

using System.Numerics;
using Rationals;

public sealed class BalancerThorne : Balancer
{
    private List<BigInteger[]>? _independentReactions;

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

    public BalancerThorne(String equation) : base(equation)
    {
    }

    public override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.VectorsNotation) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null) return "<FAIL>";

        return NumberOfIndependentReactions
             + ":"
             + String.Join(separator: ", ", _independentReactions.Select(selector: static v => '{' + String.Join(separator: ", ", v) + '}'));
    }

    protected override void BalanceImplementation()
    {
        BalancerException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");

        Rational[,] inverse;
        {
            var reduced = Equation.CompositionMatrixReduced;
            var (rowCount, columnCount) = (reduced.GetLength(dimension: 0), reduced.GetLength(dimension: 1));
            BalancerException.ThrowIf(rowCount >= columnCount, message: "The method fails on this kind of equations");

            var square = new Rational[columnCount, columnCount];
            for (var r = 0; r < columnCount; r++)
            {
                for (var c = 0; c < columnCount; c++)
                {
                    square[r, c] = r < rowCount ? reduced[r, c] : 0;
                }
            }

            for (var r = 0; r < columnCount - rowCount; r++)
            {
                square[rowCount + r, rowCount + r] = 1;
            }

            inverse = RationalArrayOperations.GetInverse(square);
        }

        Details.AddRange(inverse.ToString(title: "Inverse of the augmented matrix"));

        _independentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                          .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                          .ToList();
    }
}
