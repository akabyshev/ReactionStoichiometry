using System.Diagnostics.CodeAnalysis;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal abstract class AbstractReducibleMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    private T[,] _data;

    protected AbstractReducibleMatrix(Matrix<double> matrix, Func<double, T> convert)
    {
        _data = new T[matrix.RowCount, matrix.ColumnCount];
        CopyValues(_data, matrix.ToArray(), convert);
    }

    public int RowCount => _data.GetLength(0);
    public int ColumnCount => _data.GetLength(1);

    protected BasicOperations Basics { get; init; }

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
        for (var i_c = 0; i_c < ColumnCount; i_c++) result[i_c] = _data[i_r, i_c];

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
                if (i_c < ColumnCount - 1) sb.Append('\t');
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
            array[r, c] = convert(source[r, c]);
    }

    protected void ReduceToTrimmedRREF()
    {
        var i_c_lead = 0;
        for (var i_r = 0; i_r < RowCount; i_r++)
        {
            if (ColumnCount <= i_c_lead)
                break;

            var i = i_r;

            while (!Basics.IsNonZero(_data[i, i_c_lead]))
            {
                i++;

                if (i < RowCount)
                    continue;

                i = i_r;

                if (i_c_lead < ColumnCount - 1)
                    i_c_lead++;
                else
                    break;
            }

            if (i != i_r)
                for (var i_c = 0; i_c < ColumnCount; i_c++)
                    (_data[i_r, i_c], _data[i, i_c]) = (_data[i, i_c], _data[i_r, i_c]);

            var div = _data[i_r, i_c_lead];
            if (Basics.IsNonZero(div))
                for (var i_c = 0; i_c < ColumnCount; i_c++)
                    _data[i_r, i_c] = Basics.Divide(_data[i_r, i_c], div);

            for (var j = 0; j < RowCount; j++)
                if (j != i_r)
                {
                    var sub = _data[j, i_c_lead];
                    for (var k = 0; k < ColumnCount; k++)
                        _data[j, k] = Basics.Subtract(_data[j, k], Basics.Multiply(sub, _data[i_r, k]));
                }

            i_c_lead++;
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