using MathNet.Numerics.LinearAlgebra;
using Rationals;
using System.Text;

namespace ReactionStoichiometry
{
    internal struct Basics_<T>
    {
        internal Func<T, T, T> Add;
        internal Func<T, T, T> Subtract;
        internal Func<T, T, T> Multiply;
        internal Func<T, T, T> Divide;
        internal Func<T, bool> IsNonZero;
        internal Func<T, string> AsString;
    };

    internal abstract class CoreMatrix<T> where T : struct, IEquatable<T>, IFormattable
    {
        protected T[,] data;
        public int RowCount { get { return data.GetLength(0); } }
        public int ColumnCount { get { return data.GetLength(1); } }

        public CoreMatrix(Matrix<double> matrix, Func<double, T> convert)
        {
            data = new T[matrix.RowCount, matrix.ColumnCount];
            CopyValues(data, matrix.ToArray(), convert);
        }

        private int CountNonZeroesInRow(int i_r)
        {
            return Enumerable.Range(0, ColumnCount).Count(i => Basics.IsNonZero(data[i_r, i]));
        }

        internal int CountNonZeroesInColumn(int i_c)
        {
            return Enumerable.Range(0, RowCount).Count(i => Basics.IsNonZero(data[i, i_c]));
        }

        public T[] GetRow(int i_r)
        {
            T[] result = new T[ColumnCount];
            for (int i_c = 0; i_c < ColumnCount; i_c++)
            {
                result[i_c] = data[i_r, i_c];
            }

            return result;
        }

        public T[,] ToArray()
        {
            T[,] result = new T[RowCount, ColumnCount];
            for (int i_r = 0; i_r < RowCount; i_r++)
                for (int i_c = 0; i_c < ColumnCount; i_c++)
                    result[i_r, i_c] = data[i_r, i_c];

            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int i_r = 0; i_r < RowCount; i_r++)
            {
                for (int i_c = 0; i_c < ColumnCount; i_c++)
                {
                    sb.Append(Basics.AsString(data[i_r, i_c]));
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
            return Matrix<T>.Build.DenseOfArray(data);
        }

        protected static void CopyValues<T2>(T[,] array, T2[,] source, Func<T2, T> convert)
        {
            for (int r = 0; r < array.GetLength(0); r++)
            {
                for (int c = 0; c < array.GetLength(1); c++)
                {
                    array[r, c] = convert(source[r, c]);
                }
            }
        }

        protected Basics_<T> Basics { get; init; }

        protected void ReduceToTrimmedRREF()
        {
            int lead = 0;
            for (int r = 0; r < RowCount; r++)
            {
                if (ColumnCount <= lead)
                    break;

                int i = r;

                while (!Basics.IsNonZero(data[i, lead]))
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
                    for (int i_c = 0; i_c < ColumnCount; i_c++)
                    {
                        (data[r, i_c], data[i, i_c]) = (data[i, i_c], data[r, i_c]);
                    }
                }

                var div = data[r, lead];
                if (Basics.IsNonZero(div))
                {
                    for (int j = 0; j < ColumnCount; j++)
                        data[r, j] = Basics.Divide(data[r, j], div);
                }

                for (int j = 0; j < RowCount; j++)
                {
                    if (j != r)
                    {
                        var sub = data[j, lead];
                        for (int k = 0; k < ColumnCount; k++)
                            data[j, k] = Basics.Subtract(data[j, k], Basics.Multiply(sub, data[r, k]));
                    }
                }

                lead++;
            }

            while (CountNonZeroesInRow(RowCount - 1) == 0)
            {
                T[,] newData = new T[RowCount - 1, ColumnCount];
                CopyValues(newData, data, r => r);
                data = newData;
            }
        }
    }

    internal class RationalMatrix : CoreMatrix<Rational>
    {
        public RationalMatrix(Matrix<double> matrix) : base(matrix, x => Rational.ParseDecimal(x.ToString()))
        {
            Basics = new Basics_<Rational>
            {
                Add = Rational.Add,
                Subtract = Rational.Subtract,
                Multiply = Rational.Multiply,
                Divide = Rational.Divide,
                IsNonZero = (r => !r.IsZero),
                AsString = (r => r.ToString("C"))
            };
        }
    }

    internal class DoubleMatrix : CoreMatrix<double>
    {
        public DoubleMatrix(Matrix<double> matrix) : base(matrix, x => x)
        {
            Basics = new Basics_<double>
            {
                Add = ((a, b) => a + b),
                Subtract = ((a, b) => a - b),
                Multiply = ((a, b) => a * b),
                Divide = ((a, b) => a / b),
                IsNonZero = (d => Math.Abs(d) > Helpers.FP_TOLERANCE),
                AsString = (d => d.ToString())
            };
        }
    }
}