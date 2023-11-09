namespace ReactionStoichiometry;

using System.Numerics;
using Rationals;

public sealed class BalancerThorne : Balancer
{
    private List<BigInteger[]>? _independentReactions;

    internal Int32 NumberOfIndependentReactions => _independentReactions!.Count;

    public BalancerThorne(String equation) : base(equation)
    {
    }

    public override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.Vectors) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null) return "<FAIL>";

        return NumberOfIndependentReactions
             + ":"
             + String.Join(separator: ", ", _independentReactions.Select(selector: static v => '{' + String.Join(separator: ", ", v) + '}'));
    }

    protected override IEnumerable<String> Outcome()
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null) return new[] { "<FAIL>" };

        return _independentReactions.Select(EquationWithIntegerCoefficients);
    }

    protected override void Balance()
    {
        BalancerException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");

        Rational[,] inverse;
        {
            BalancerException.ThrowIf(Equation.MagicMatrix.RowCount() >= Equation.MagicMatrix.ColumnCount(), message: "The method fails on this kind of equations");

            var square = new Rational[Equation.MagicMatrix.ColumnCount(), Equation.MagicMatrix.ColumnCount()];
            for (var r = 0; r < Equation.MagicMatrix.ColumnCount(); r++)
            {
                for (var c = 0; c < Equation.MagicMatrix.ColumnCount(); c++)
                {
                    square[r, c] = r < Equation.MagicMatrix.RowCount() ? Equation.MagicMatrix[r, c] : 0;
                }
            }

            for (var r = 0; r < Equation.MagicMatrix.ColumnCount() - Equation.MagicMatrix.RowCount(); r++)
            {
                square[Equation.MagicMatrix.RowCount() + r, Equation.MagicMatrix.RowCount() + r] = 1;
            }

            inverse = RationalArrayOperations.GetInverse(square);
        }

        Details.AddRange(inverse.ToString(title: "Inverse"));

        _independentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                          .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                          .ToList();
    }
}
