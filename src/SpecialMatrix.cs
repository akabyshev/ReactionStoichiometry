namespace ReactionStoichiometry;

using System.Text;
using MathNet.Numerics.LinearAlgebra;

internal abstract class SpecialMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    private protected T[,] Data;

    public Int32 RowCount => Data.GetLength(0);
    public Int32 ColumnCount => Data.GetLength(1);

    protected BasicOperations Basics { get; init; }

    protected SpecialMatrix(Matrix<Double> matrix, Func<Double, T> convert)
    {
        Data = new T[matrix.RowCount, matrix.ColumnCount];
        CopyValues(Data, matrix.ToArray(), convert);
    }

    protected Int32 CountNonZeroesInRow(Int32 r) => Enumerable.Range(0, ColumnCount).Count(i => Basics.IsNonZero(Data[r, i]));

    internal Int32 CountNonZeroesInColumn(Int32 c) => Enumerable.Range(0, RowCount).Count(i => Basics.IsNonZero(Data[i, c]));

    public T[] GetRow(Int32 r)
    {
        var result = new T[ColumnCount];
        for (var c = 0; c < ColumnCount; c++)
        {
            result[c] = Data[r, c];
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
                result[r, c] = Data[r, c];
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
                sb.Append(Basics.AsString(Data[r, c]));
                if (c < ColumnCount - 1) sb.Append('\t');
            }

            sb.Append('\n');
        }

        return sb.ToString();
    }

    public Matrix<T> ToMatrix() => Matrix<T>.Build.DenseOfArray(Data);

    protected static void CopyValues<T2>(T[,] array, T2[,] source, Func<T2, T> convert)
    {
        for (var r = 0; r < array.GetLength(0); r++)
        {
            for (var c = 0; c < array.GetLength(1); c++)
            {
                array[r, c] = convert(source[r, c]);
            }
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