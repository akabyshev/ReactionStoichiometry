using MathNet.Numerics.LinearAlgebra;
using System.Text;

namespace ReactionStoichiometry;

internal abstract class AbstractReducibleMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    protected struct BasicOperations
    {
        internal Func<T, T, T> Add;
        internal Func<T, T, T> Subtract;
        internal Func<T, T, T> Multiply;
        internal Func<T, T, T> Divide;
        internal Func<T, bool> IsNonZero;
        internal Func<T, string> AsString;
    };

    private T[,] _data;
    public int RowCount => _data.GetLength(0);
    public int ColumnCount => _data.GetLength(1);

    protected AbstractReducibleMatrix(Matrix<double> matrix, Func<double, T> convert)
    {
        _data = new T[matrix.RowCount, matrix.ColumnCount];
        CopyValues(_data, matrix.ToArray(), convert);
    }

    private int CountNonZeroesInRow(int i_r)
    {
        return Enumerable.Range(0, ColumnCount).Count(i => Basics.IsNonZero(_data[i_r, i]));
    }

    internal int CountNonZeroesInColumn(int i_c)
    {
        return Enumerable.Range(0, RowCount).Count(i => Basics.IsNonZero(_data[i, i_c]));
    }

    public T[] GetRow(int i_r)
    {
        var result = new T[ColumnCount];
        for (var i_c = 0; i_c < ColumnCount; i_c++)
        {
            result[i_c] = _data[i_r, i_c];
        }

        return result;
    }

    public T[,] ToArray()
    {
        var result = new T[RowCount, ColumnCount];
        for (var i_r = 0; i_r < RowCount; i_r++)
            for (var i_c = 0; i_c < ColumnCount; i_c++)
                result[i_r, i_c] = _data[i_r, i_c];

        return result;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        for (var i_r = 0; i_r < RowCount; i_r++)
        {
            for (var i_c = 0; i_c < ColumnCount; i_c++)
            {
                sb.Append(Basics.AsString(_data[i_r, i_c]));
                if (i_c < ColumnCount - 1)
                {
                    sb.Append('\t');
                }
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
        {
            for (var c = 0; c < array.GetLength(1); c++)
            {
                array[r, c] = convert(source[r, c]);
            }
        }
    }

    protected BasicOperations Basics { get; init; }

    protected void ReduceToTrimmedRREF()
    {
        var lead = 0;
        for (var r = 0; r < RowCount; r++)
        {
            if (ColumnCount <= lead)
                break;

            var i = r;

            while (!Basics.IsNonZero(_data[i, lead]))
            {
                i++;

                if (i >= RowCount)
                {
                    i = r;

                    if (lead < ColumnCount - 1)
                        lead++;
                    else break;
                }
            }

            if (i != r)
            {
                for (var i_c = 0; i_c < ColumnCount; i_c++)
                {
                    (_data[r, i_c], _data[i, i_c]) = (_data[i, i_c], _data[r, i_c]);
                }
            }

            var div = _data[r, lead];
            if (Basics.IsNonZero(div))
            {
                for (var j = 0; j < ColumnCount; j++)
                    _data[r, j] = Basics.Divide(_data[r, j], div);
            }

            for (var j = 0; j < RowCount; j++)
            {
                if (j != r)
                {
                    var sub = _data[j, lead];
                    for (var k = 0; k < ColumnCount; k++)
                        _data[j, k] = Basics.Subtract(_data[j, k], Basics.Multiply(sub, _data[r, k]));
                }
            }

            lead++;
        }

        while (CountNonZeroesInRow(RowCount - 1) == 0)
        {
            var newData = new T[RowCount - 1, ColumnCount];
            CopyValues(newData, _data, r => r);
            _data = newData;
        }
    }
}