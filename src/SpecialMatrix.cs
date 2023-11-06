namespace ReactionStoichiometry;

using MathNet.Numerics.LinearAlgebra;

internal abstract class SpecialMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    private protected readonly BasicOperations Basics;
    internal T[,] Data { get; private protected set; }

    protected SpecialMatrix(Int32 rows, Int32 columns, BasicOperations basics)
    {
        Basics = basics;
        Data = new T[rows, columns];
    }

    protected SpecialMatrix(Matrix<Double> matrix, Func<Double, T> convert, BasicOperations basics)
    {
        Basics = basics;

        var source = matrix.ToArray();
        Data = new T[source.GetLength(0), source.GetLength(1)];
        for (var r = 0; r < Data.GetLength(0); r++)
        {
            for (var c = 0; c < Data.GetLength(1); c++)
            {
                Data[r, c] = convert(source[r, c]);
            }
        }
    }

    internal Int32 RowCount => Data.GetLength(0);
    internal Int32 ColumnCount => Data.GetLength(1);

    internal Boolean IsIdentityMatrix
    {
        get
        {
            if (RowCount != ColumnCount) return false;

            for (var r = 0; r < RowCount; r++)
            {
                for (var c = 0; c < ColumnCount; c++)
                {
                    if (r == c && !Basics.IsOne(Data[r, c])) return false;
                    if (r != c && !Basics.IsZero(Data[r, c])) return false;
                }
            }
            return true;
        }
    }

    // todo: get rid of this
    internal IEnumerable<T> GetRow(Int32 r)
    {
        var result = new T[ColumnCount];
        for (var c = 0; c < ColumnCount; c++)
        {
            result[c] = Data[r, c];
        }

        return result;
    }

    // todo: get rid of this
    internal Boolean IsColumnAllZeroes(Int32 c)
    {
        for (var r = 0; r < RowCount; r++)
        {
            if (!Basics.IsZero(Data[r, c])) return false;
        }
        return true;
    }

    #region Nested type: BasicOperations
    internal struct BasicOperations
    {
        // ReSharper disable once NotAccessedField.Global
        internal Func<T, T, T> Add;
        internal Func<T, T, T> Subtract;
        internal Func<T, T, T> Multiply;
        internal Func<T, T, T> Divide;
        internal Func<T, Boolean> IsZero;
        internal Func<T, Boolean> IsOne;
        internal Func<T, String> AsString;
    }
    #endregion
}

