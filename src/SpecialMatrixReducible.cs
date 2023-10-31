namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

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

            while (!Basics.IsNonZero(Data[i, leadColumnIndex]))
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
            if (Basics.IsNonZero(div))
                for (var c = 0; c < ColumnCount; c++)
                {
                    Data[r, c] = Basics.Divide(Data[r, c], div);
                }

            for (var j = 0; j < RowCount; j++)
            {
                if (j == r) continue;

                var sub = Data[j, leadColumnIndex];
                for (var k = 0; k < ColumnCount; k++)
                {
                    Data[j, k] = Basics.Subtract(Data[j, k], Basics.Multiply(sub, Data[r, k]));
                }
            }

            leadColumnIndex++;
        }

        while (CountNonZeroesInRow(RowCount - 1) == 0)
        {
            var newData = new T[RowCount - 1, ColumnCount];
            CopyValues(newData, Data, r => r);
            Data = newData;
        }
    }
}