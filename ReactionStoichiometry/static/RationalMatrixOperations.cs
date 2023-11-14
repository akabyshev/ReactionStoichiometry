using Rationals;

namespace ReactionStoichiometry
{
    internal static partial class RationalMatrixOperations
    {
        internal static Int32 RowCount(this Rational[,] me)
        {
            return me.GetLength(dimension: 0);
        }

        internal static Boolean IsIdentityMatrix(this Rational[,] me)
        {
            if (me.RowCount() != me.ColumnCount())
            {
                return false;
            }

            for (var r = 0; r < me.RowCount(); r++)
            {
                for (var c = 0; c < me.ColumnCount(); c++)
                {
                    if (r == c && !me[r, c].IsOne)
                    {
                        return false;
                    }
                    if (r != c && !me[r, c].IsZero)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        internal static Int32 ColumnCount(this Rational[,] me)
        {
            return me.GetLength(dimension: 1);
        }

        internal static Rational[] Row(this Rational[,] me, Int32 r)
        {
            var result = new Rational[me.ColumnCount()];
            for (var c = 0; c < me.ColumnCount(); c++)
            {
                result[c] = me[r, c];
            }

            return result;
        }

        internal static Rational[] Column(this Rational[,] me, Int32 c)
        {
            var result = new Rational[me.RowCount()];
            for (var r = 0; r < me.RowCount(); r++)
            {
                result[r] = me[r, c];
            }

            return result;
        }

        internal static String Readable(this Rational[,] me, String title, Func<Int32, String>? rowHeaders = null, Func<Int32, String>? columnHeaders = null)
        {
            rowHeaders ??= static i => $"R#{i + 1}";

            List<String> result = new() { $"[[{title}]]" };

            if (columnHeaders != null)
            {
                List<String> line = new() { String.Empty };
                line.AddRange(Enumerable.Range(start: 0, me.ColumnCount()).Select(columnHeaders));
                result.Add(String.Join(separator: '\t', line));
            }

            for (var r = 0; r < me.RowCount(); r++)
            {
                List<String> line = new() { rowHeaders(r) };
                line.AddRange(Enumerable.Range(start: 0, me.ColumnCount()).Select(selector: c => me[r, c].ToString()!));
                result.Add(String.Join(separator: '\t', line));
            }

            return String.Join(Environment.NewLine, result);
        }
    }
}
