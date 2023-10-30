namespace ReactionStoichiometry;

using System.Text;
using MathNet.Numerics.LinearAlgebra;

internal abstract class AbstractSpecialReducibleMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    private T[,] _data;

    public Int32 RowCount => _data.GetLength(0);
    public Int32 ColumnCount => _data.GetLength(1);

    protected BasicOperations Basics { get; init; }

    protected AbstractSpecialReducibleMatrix(Matrix<Double> matrix, Func<Double, T> convert)
    {
        _data = new T[matrix.RowCount, matrix.ColumnCount];
        CopyValues(_data, matrix.ToArray(), convert);
    }

    private Int32 CountNonZeroesInRow(Int32 r) => Enumerable.Range(0, ColumnCount).Count(i => Basics.IsNonZero(_data[r, i]));

    internal Int32 CountNonZeroesInColumn(Int32 c) => Enumerable.Range(0, RowCount).Count(i => Basics.IsNonZero(_data[i, c]));

    public T[] GetRow(Int32 r)
    {
        var result = new T[ColumnCount];
        for (var c = 0; c < ColumnCount; c++)
        {
            result[c] = _data[r, c];
        }

        return result;
    }

    public T[,] ToArray()
    {
        var result = new T[RowCount, ColumnCount];
        for (var r = 0; r < RowCount; r++)
        {
            for (var c = 0; c < ColumnCount; c++)
            {
                result[r, c] = _data[r, c];
            }
        }

        return result;
    }

    public override String ToString()
    {
        StringBuilder sb = new();

        for (var r = 0; r < RowCount; r++)
        {
            for (var c = 0; c < ColumnCount; c++)
            {
                sb.Append(Basics.AsString(_data[r, c]));
                if (c < ColumnCount - 1) sb.Append('\t');
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }

    public Matrix<T> ToMatrix() => Matrix<T>.Build.DenseOfArray(_data);

    private static void CopyValues<T2>(T[,] array, T2[,] source, Func<T2, T> convert)
    {
        for (var r = 0; r < array.GetLength(0); r++)
        {
            for (var c = 0; c < array.GetLength(1); c++)
            {
                array[r, c] = convert(source[r, c]);
            }
        }
    }

    protected void Reduce()
    {
        var leadColumnIndex = 0;
        for (var r = 0; r < RowCount; r++)
        {
            if (ColumnCount <= leadColumnIndex) break;

            var i = r;

            while (!Basics.IsNonZero(_data[i, leadColumnIndex]))
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
                    (_data[r, c], _data[i, c]) = (_data[i, c], _data[r, c]);
                }

            var div = _data[r, leadColumnIndex];
            if (Basics.IsNonZero(div))
                for (var c = 0; c < ColumnCount; c++)
                {
                    _data[r, c] = Basics.Divide(_data[r, c], div);
                }

            for (var j = 0; j < RowCount; j++)
            {
                if (j == r) continue;

                var sub = _data[j, leadColumnIndex];
                for (var k = 0; k < ColumnCount; k++)
                {
                    _data[j, k] = Basics.Subtract(_data[j, k], Basics.Multiply(sub, _data[r, k]));
                }
            }

            leadColumnIndex++;
        }

        while (CountNonZeroesInRow(RowCount - 1) == 0)
        {
            var newData = new T[RowCount - 1, ColumnCount];
            CopyValues(newData, _data, r => r);
            _data = newData;
        }
    }

    protected struct BasicOperations
    {
        // ReSharper disable once NotAccessedField.Global
        internal Func<T, T, T> Add;
        internal Func<T, T, T> Subtract;
        internal Func<T, T, T> Multiply;
        internal Func<T, T, T> Divide;
        internal Func<T, Boolean> IsNonZero;
        internal Func<T, String> AsString;
    }
}