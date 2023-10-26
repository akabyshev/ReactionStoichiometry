using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal abstract class AbstractReducibleMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    private T[,] _data;

    public int RowCount => _data.GetLength(0);
    public int ColumnCount => _data.GetLength(1);

    protected BasicOperations Basics { get; init; }

    protected AbstractReducibleMatrix(Matrix<double> matrix, Func<double, T> convert)
    {
        _data = new T[matrix.RowCount, matrix.ColumnCount];
        CopyValues(_data, matrix.ToArray(), convert);
    }

    private int CountNonZeroesInRow(int r)
    {
        return Enumerable.Range(0, ColumnCount).Count(i => Basics.IsNonZero(_data[r, i]));
    }

    internal int CountNonZeroesInColumn(int c)
    {
        return Enumerable.Range(0, RowCount).Count(i => Basics.IsNonZero(_data[i, c]));
    }

    public T[] GetRow(int r)
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
        for (var c = 0; c < ColumnCount; c++)
        {
            result[r, c] = _data[r, c];
        }

        return result;
    }

    public override string ToString()
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

    public Matrix<T> ToMatrix()
    {
        return Matrix<T>.Build.DenseOfArray(_data);
    }

    private static void CopyValues<T2>(T[,] array, T2[,] source, Func<T2, T> convert)
    {
        for (var r = 0; r < array.GetLength(0); r++)
        for (var c = 0; c < array.GetLength(1); c++)
        {
            array[r, c] = convert(source[r, c]);
        }
    }

    protected void Reduce()
    {
        var leadColumnIndex = 0;
        for (var r = 0; r < RowCount; r++)
        {
            if (ColumnCount <= leadColumnIndex)
                break;

            var i = r;

            while (!Basics.IsNonZero(_data[i, leadColumnIndex]))
            {
                i++;

                if (i < RowCount)
                    continue;

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
                if (j == r)
                    continue;
                
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
        internal Func<T, T, T> Add;
        internal Func<T, T, T> Subtract;
        internal Func<T, T, T> Multiply;
        internal Func<T, T, T> Divide;
        internal Func<T, bool> IsNonZero;
        internal Func<T, string> AsString;
    }
}