namespace ReactionStoichiometry;

using Rationals;

internal sealed class RationalMatrix
{
    private readonly Rational[,] _data;

    private RationalMatrix(Rational[,] data) => _data = data;

    internal Int32 RowCount => _data.GetLength(0);
    internal Int32 ColumnCount => _data.GetLength(1);
    internal Int32 Nullity => ColumnCount - Rank;
    internal Int32 Rank { get; private set; }

    internal Boolean IsIdentityMatrix
    {
        get
        {
            if (RowCount != ColumnCount) return false;

            for (var r = 0; r < RowCount; r++)
            {
                for (var c = 0; c < ColumnCount; c++)
                {
                    if (r == c && !_data[r, c].IsOne) return false;
                    if (r != c && !_data[r, c].IsZero) return false;
                }
            }
            return true;
        }
    }

    internal Rational this[Int32 r, Int32 c] => _data[r, c];

    public static RationalMatrix CreateInstance<TSource>(TSource[,] source, Func<TSource, Rational> convert)
    {
        var result = new RationalMatrix(new Rational[source.GetLength(0), source.GetLength(1)]);
        for (var r = 0; r < result._data.GetLength(0); r++)
        {
            for (var c = 0; c < result._data.GetLength(1); c++)
            {
                result._data[r, c] = convert(source[r, c]);
            }
        }

        result.Rank = RationalMatrixMath.GetRank(result._data);
        return result;
    }

    internal IEnumerable<String> PrettyPrint(String title, Func<Int32, String>? columnHeaders = null)
    {
        List<String> result = new() { $"[[{title}]]" };

        List<String> line = new();
        if (columnHeaders != null)
        {
            line.Add(String.Empty);
            line.AddRange(Enumerable.Range(0, ColumnCount).Select(columnHeaders));
            result.Add(String.Join('\t', line));
        }

        for (var r = 0; r < RowCount; r++)
        {
            line.Clear();
            line.Add($"R#{r + 1}");

            for (var c = 0; c < ColumnCount; c++)
            {
                line.Add(this[r, c].ToString("C"));
            }

            result.Add(String.Join('\t', line));
        }

        return result;
    }

    internal Rational[] GetRow(Int32 r)
    {
        var result = new Rational[ColumnCount];
        for (var c = 0; c < ColumnCount; c++)
        {
            result[c] = _data[r, c];
        }

        return result;
    }

    internal Rational[] GetColumn(Int32 c)
    {
        var result = new Rational[RowCount];
        for (var r = 0; r < RowCount; r++)
        {
            result[r] = _data[r, c];
        }

        return result;
    }

    internal Boolean IsColumnAllZeroes(Int32 c)
    {
        for (var r = 0; r < RowCount; r++)
        {
            if (!_data[r, c].IsZero) return false;
        }
        return true;
    }

    internal Rational[,] Reduce() => RationalMatrixMath.GetReduced(_data);

    internal Rational Determinant() => RationalMatrixMath.GetDeterminant(_data);

    internal RationalMatrix Inverse() => CreateInstance(RationalMatrixMath.GetInverse(_data), static r => r);
}
