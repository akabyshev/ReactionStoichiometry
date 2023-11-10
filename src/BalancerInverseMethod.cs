namespace ReactionStoichiometry;

using System.Numerics;
using Rationals;

public sealed class BalancerInverseMethod : Balancer
{
    private List<BigInteger[]>? _independentReactions;

    internal Int32 NumberOfIndependentReactions => _independentReactions!.Count;

    public BalancerInverseMethod(String equation) : base(equation)
    {
    }

    public override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.Vectors)
            return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null)
            return "<FAIL>";

        return NumberOfIndependentReactions
             + ":"
             + String.Join(separator: ", ", _independentReactions.Select(selector: static v => '{' + String.Join(separator: ", ", v) + '}'));
    }

    protected override IEnumerable<String> Outcome()
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_independentReactions == null)
            return new[] { "<FAIL>" };

        return _independentReactions.Select(EquationWithIntegerCoefficients);
    }

    protected override void Balance()
    {
        AppSpecificException.ThrowIf(Equation.CompositionMatrixNullity == 0, message: "Zero null-space");

        Rational[,] inverse;
        {
            AppSpecificException.ThrowIf(Equation.REF.RowCount() >= Equation.REF.ColumnCount(), message: "The method fails on this kind of equations");

            var square = new Rational[Equation.REF.ColumnCount(), Equation.REF.ColumnCount()];
            Array.Copy(Equation.REF, square, Equation.REF.Length);
            for (var r = Equation.REF.RowCount(); r < square.RowCount(); r++)
            {
                for (var c = 0; c < square.ColumnCount(); c++)
                    square[r, c] = r == c ? 1 : 0;
            }

            inverse = RationalMatrixOperations.GetInverse(square);
        }

        _independentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                          .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                          .ToList();
    }
}
