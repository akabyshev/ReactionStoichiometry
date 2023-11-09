namespace ReactionStoichiometry;

using System.Diagnostics;
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
            BalancerException.ThrowIf(Equation.REF.RowCount() >= Equation.REF.ColumnCount(), message: "The method fails on this kind of equations");

            var square = new Rational[Equation.REF.ColumnCount(), Equation.REF.ColumnCount()];
            Array.Copy(Equation.REF, square, Equation.REF.Length);
            for (var r = Equation.REF.RowCount(); r < square.RowCount(); r++)
            {
                for (var c = 0; c < square.ColumnCount(); c++)
                {
                    square[r, c] = r == c ? 1 : 0;
                }
            }

            inverse = RationalArrayOperations.GetInverse(square);

            Debug.WriteLine(square.ToString(title: "Square"));
            Debug.WriteLine(inverse.ToString(title: "Inverse"));
        }

        _independentReactions = Enumerable.Range(inverse.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                                          .Select(selector: c => inverse.Column(c).ScaleToIntegers())
                                          .ToList();
    }
}
