namespace ReactionStoichiometry;

using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using Rationals;

internal abstract class SpecialMatrixReducible<T> : SpecialMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    protected SpecialMatrixReducible(Matrix<Double> matrix, Func<Double, T> convert) : base(matrix, convert)
    {
    }

    protected void Reduce()
    {
        var leadColumnIndex = 0;
        for (var r = 0; r < RowCount; r++)
        {
            if (ColumnCount <= leadColumnIndex) break;

            var i = r;

            while (Basics.IsZero(Data[i, leadColumnIndex]))
            {
                i++;

                if (i < RowCount) continue;

                i = r;

                if (leadColumnIndex < ColumnCount - 1)
                    leadColumnIndex++;
                else
                    break;
            }

            if (i != r)
                for (var c = 0; c < ColumnCount; c++)
                {
                    (Data[r, c], Data[i, c]) = (Data[i, c], Data[r, c]);
                }

            var div = Data[r, leadColumnIndex];
            if (!Basics.IsZero(div))
                for (var c = 0; c < ColumnCount; c++)
                {
                    Data[r, c] = Basics.Divide(Data[r, c], div);
                }

            for (var r2 = 0; r2 < RowCount; r2++)
            {
                if (r2 == r) continue;

                var sub = Data[r2, leadColumnIndex];
                for (var c2 = 0; c2 < ColumnCount; c2++)
                {
                    Data[r2, c2] = Basics.Subtract(Data[r2, c2], Basics.Multiply(sub, Data[r, c2]));
                }
            }

            leadColumnIndex++;
        }

        var bottomZeroRows = Enumerable.Range(0, RowCount).Reverse().TakeWhile(r => CountNonZeroesInRow(r) == 0).Count();

        var dataNoZeroRows = new T[RowCount - bottomZeroRows, ColumnCount];
        CopyValues(dataNoZeroRows, Data, static r => r);
        Data = dataNoZeroRows;
    }
}

internal sealed class SpecialMatrixReducedDouble : SpecialMatrixReducible<Double>
{
    private SpecialMatrixReducedDouble(Matrix<Double> matrix) : base(matrix, static x => x) =>
        Basics = new BasicOperations
                 {
                     Add = static (d1, d2) => d1 + d2,
                     Subtract = static (d1, d2) => d1 - d2,
                     Multiply = static (d1, d2) => d1 * d2,
                     Divide = static (d1, d2) => d1 / d2,
                     IsZero = static d => !Utils.IsNonZeroDouble(d),
                     IsOne = static d => !Utils.IsNonZeroDouble(1.0d - d),
                     AsString = static d => d.ToString(CultureInfo.InvariantCulture)
                 };

    internal static SpecialMatrixReducedDouble CreateInstance(Matrix<Double> matrix)
    {
        var result = new SpecialMatrixReducedDouble(matrix);
        result.Reduce();
        return result;
    }
}

internal sealed class SpecialMatrixReducedRational : SpecialMatrixReducible<Rational>
{
    private SpecialMatrixReducedRational(Matrix<Double> matrix) : base(matrix, static x => Rational.ParseDecimal(x.ToString(CultureInfo.InvariantCulture))) =>
        Basics = new BasicOperations
                 {
                     Add = Rational.Add,
                     Subtract = Rational.Subtract,
                     Multiply = Rational.Multiply,
                     Divide = Rational.Divide,
                     IsZero = static r => r.IsZero,
                     IsOne = static r => r.IsOne,
                     AsString = static r => r.ToString("C")
                 };

    internal static SpecialMatrixReducedRational CreateInstance(Matrix<Double> matrix)
    {
        var result = new SpecialMatrixReducedRational(matrix);
        result.Reduce();
        return result;
    }
}