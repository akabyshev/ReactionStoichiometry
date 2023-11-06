namespace ReactionStoichiometry;

using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Rationals;

internal abstract class SpecialMatrix<T> where T : struct, IEquatable<T>, IFormattable
{
    protected readonly BasicOperations Basics;
    protected T[,] Data;

    protected SpecialMatrix(Int32 rows, Int32 columns, BasicOperations basics)
    {
        Basics = basics;
        Data = new T[rows, columns];
    }

    protected SpecialMatrix(Matrix<Double> matrix, Func<Double, T> convert, BasicOperations basics)
    {
        Basics = basics;
        Data = new T[matrix.RowCount, matrix.ColumnCount];
        CopyValues(Data, matrix.ToArray(), convert);
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

    internal IEnumerable<T> GetRow(Int32 r)
    {
        var result = new T[ColumnCount];
        for (var c = 0; c < ColumnCount; c++)
        {
            result[c] = Data[r, c];
        }

        return result;
    }

    internal T[,] ToArray()
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

    internal Matrix<T> ToMatrix() => Matrix<T>.Build.DenseOfArray(Data);

    //internal Int32 CountNonZeroesInRow(Int32 r) => Enumerable.Range(0, ColumnCount).Count(i => !Basics.IsZero(Data[r, i]));
    // todo: replace to Any() and delete both?
    internal Int32 CountNonZeroesInColumn(Int32 c) => Enumerable.Range(0, RowCount).Count(i => !Basics.IsZero(Data[i, c]));

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

internal sealed class DefinedBasicOperations
{
    internal static SpecialMatrix<Double>.BasicOperations BasicOperationsOfDouble =>
        new()
        {
            Add = static (d1, d2) => d1 + d2,
            Subtract = static (d1, d2) => d1 - d2,
            Multiply = static (d1, d2) => d1 * d2,
            Divide = static (d1, d2) => d1 / d2,
            IsZero = static d => Utils.IsZeroDouble(d),
            IsOne = static d => Utils.IsZeroDouble(1.0d - d),
            AsString = static d => d.ToString()
        };

    internal static SpecialMatrix<Rational>.BasicOperations BasicOperationsOfRational =>
        new()
        {
            Add = Rational.Add,
            Subtract = Rational.Subtract,
            Multiply = Rational.Multiply,
            Divide = Rational.Divide,
            IsZero = static r => r.IsZero,
            IsOne = static r => r.IsOne,
            AsString = static r => r.ToString("C")
        };
}