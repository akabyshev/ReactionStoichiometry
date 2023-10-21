using MathNet.Numerics.LinearAlgebra;
using Rationals;
using System.Text.RegularExpressions;

namespace ReactionStoichiometry
{
    internal abstract class Balancer<T_RREF>
    {
        public string Outcome { get; protected set; }
        public string Skeletal { get; init; }
        protected int ReactantsCount { get; init; }

        protected readonly List<string> fragments = new();
        protected readonly Matrix<double> matrix;

        protected const string MULTIPLICATION_SYMBOL = "·";
        protected Func<int, string> Labeller { get { return fragments.Count <= 7 ? Helpers.LetterLabel : Helpers.GenericLabel; } }

        protected readonly List<string> details = new();
        public string Details { get { return string.Join(Environment.NewLine, details); } }

        protected readonly List<string> diagnostics = new();
        public string Diagnostics { get { return string.Join(Environment.NewLine, diagnostics); } }

        public Balancer(string equation)
        {
            Outcome = "<FAIL>";
            Skeletal = equation.Replace(" ", "");
            ReactantsCount = Skeletal.Split('=')[0].Split('+').Length;

            string[] chargeSymbols = new string[] { "Qn", "Qp" };
            var chargeParsingStrings = new string[][]
            {
                new string[] { "Qn", @"Qn(\d*)$", "{$1-}" },
                new string[] { "Qp", @"Qp(\d*)$", "{$1+}" },
            };

            fragments.AddRange(Regex.Split(Skeletal, REGEX.AllowedDividers));

            try
            {
                List<string> elements = new();
                elements.AddRange(Regex.Matches(Skeletal, REGEX.ElementSymbol).Select(m => m.Value).Concat(chargeSymbols).Distinct());
                elements.Add("{e}");

                matrix = Matrix<double>.Build.Dense(elements.Count, fragments.Count);
                for (int i_r = 0; i_r < elements.Count; i_r++)
                {
                    Regex regex = new(REGEX.ElementTemplate.Replace("X", elements[i_r]));

                    for (int i_c = 0; i_c < fragments.Count; i_c++)
                    {
                        string plain_fragment = Helpers.UnfoldFragment(fragments[i_c]);
                        matrix[i_r, i_c] += regex.Matches(plain_fragment).Cast<Match>().Sum(match => double.Parse(match.Groups[1].Value));
                    }
                }

                for (int i = 0; i < chargeParsingStrings.Length; i++)
                {
                    for (int index = 0; index < fragments.Count; index++)
                    {
                        fragments[index] = Regex.Replace(fragments[index], pattern: chargeParsingStrings[i][1], replacement: chargeParsingStrings[i][2]);
                    }
                }

                var totalCharge = matrix.Row(elements.IndexOf("Qp")) - matrix.Row(elements.IndexOf("Qn"));
                matrix.SetRow(elements.IndexOf("{e}"), totalCharge);

                if (totalCharge.CountNonZeroes() == 0)
                {
                    matrix = matrix.RemoveRow(elements.IndexOf("{e}"));
                    elements.Remove("{e}");
                }

                matrix = matrix.RemoveRow(elements.IndexOf("Qn"));
                elements.Remove("Qn");
                matrix = matrix.RemoveRow(elements.IndexOf("Qp"));
                elements.Remove("Qp");

                details.AddRange(Helpers.PrettyPrintMatrix("Chemical composition matrix", matrix.ToArray(), Helpers.PrettyPrintDouble, fragments, elements));
                details.Add($"RxC: {matrix.RowCount}x{matrix.ColumnCount}, rank = {matrix.Rank()}, nullity = {matrix.Nullity()}");
            }
            catch (Exception e)
            {
                throw new BalancerException($"Parsing failed: {e.Message}");
            }

            try
            {
                Balance();
            }
            catch (BalancerException e)
            {
                diagnostics.Add($"This equation can't be balanced: {e.Message}");
            }
        }

        internal abstract void Balance();
        protected abstract long[] VectorScaler(T_RREF[] v);
        protected abstract string PrettyPrinter(T_RREF value);
    }

    internal class ThorneBalancer : Balancer<double>
    {
        public ThorneBalancer(string equation) : base(equation)
        {
        }

        internal override void Balance()
        {
            int nullity = matrix.ColumnCount - matrix.Rank();

            Matrix<double> GetAugmentedReducedMatrix()
            {
                Matrix<double> reduced = matrix.Clone();
                if ((matrix.RowCount == matrix.ColumnCount))
                {
                    var temp = new DoubleMatrixInRREF(matrix);
                    reduced = temp.ToMatrix();
                }

                if (reduced.RowCount == reduced.ColumnCount)
                {
                    throw new BalancerException("Matrix in RREF is still square");
                }

                var zeros = Matrix<double>.Build.Dense(nullity, matrix.Rank());
                var identity = Matrix<double>.Build.DenseIdentity(nullity);
                var augmented = reduced.Stack(zeros.Append(identity));

                if (augmented.HasZeroDeterminant())
                {
                    diagnostics.AddRange(Helpers.PrettyPrintMatrix("Zero-determinant matrix", augmented.ToArray(), PrettyPrinter));
                    throw new BalancerException("Matrix can't be inverted");
                }

                return augmented;
            }

            Matrix<double> AM = GetAugmentedReducedMatrix();

            Matrix<double> IAM = AM.Inverse();
            details.AddRange(Helpers.PrettyPrintMatrix("Inverse of the augmented matrix", IAM.ToArray(), PrettyPrinter));

            List<string> independent_equations = new();
            foreach (int i in Enumerable.Range(IAM.ColumnCount - nullity, nullity))
            {
                var scaled_nsv = VectorScaler(IAM.Column(i).ToArray());
                independent_equations.Add(GetEquationWithCoefficients(scaled_nsv));

                diagnostics.Add(string.Join('\t', scaled_nsv));
            }

            Outcome = string.Join(Environment.NewLine, independent_equations);
        }

        protected override long[] VectorScaler(double[] v)
        {
            return Helpers.ScaleDoubles(v);
        }

        private string GetEquationWithCoefficients(in long[] coefs)
        {
            List<string> l = new();
            List<string> r = new();

            for (int i = 0; i < fragments.Count; i++)
            {
                if (coefs[i] == 0)
                    continue;

                var coef_abs = Math.Abs(coefs[i]);
                var t = ((coef_abs == 1) ? "" : coef_abs.ToString() + MULTIPLICATION_SYMBOL) + fragments[i];
                (coefs[i] < 0 ? l : r).Add(t);
            }

            return string.Join(" + ", l) + " = " + string.Join(" + ", r);
        }

        protected override string PrettyPrinter(double value)
        {
            return Helpers.PrettyPrintDouble(value);
        }
    }

    internal abstract class RisteskiBalancer<T> : Balancer<T> where T : struct, IEquatable<T>, IFormattable
    {
        public RisteskiBalancer(string equation) : base(equation)
        {
        }

        internal override void Balance()
        {
            for (int i_c = ReactantsCount; i_c < fragments.Count; i_c++)
            {
                matrix.SetColumn(i_c, matrix.Column(i_c).Multiply(-1));
            }

            CoreMatrix<T> RAM = GetReducedAugmentedMatrix();
            details.AddRange(Helpers.PrettyPrintMatrix("RREF-data augmented matrix", RAM.ToArray(), PrettyPrinter));

            var free_var_indices = Enumerable.Range(0, RAM.ColumnCount).Where(i_c => RAM.CountNonZeroesInColumn(i_c) > 1);
            if (!free_var_indices.Any())
            {
                throw new BalancerException("This SLE is unsolvable");
            }

            List<string> expressions = new();
            for (int i_r = 0; i_r < RAM.RowCount; i_r++)
            {
                var scaled_row = VectorScaler(RAM.GetRow(i_r));
                int pivot_column = Array.FindIndex(scaled_row, i => i != 0);

                var expression_parts = new List<string>();
                for (int i_c = pivot_column + 1; i_c < scaled_row.Length; i_c++)
                {
                    if (scaled_row[i_c] != 0)
                    {
                        string coef = (-1 * scaled_row[i_c]).ToString();
                        if (coef == "1")
                        {
                            coef = string.Empty;
                        }
                        if (coef == "-1")
                        {
                            coef = "-";
                        }

                        expression_parts.Add($"{coef}{Labeller(i_c)}");
                    }
                }
                var expression = string.Join(" + ", expression_parts).Replace("+ -", "- ");

                if (scaled_row[pivot_column] != 1)
                {
                    if (expression_parts.Count > 1)
                    {
                        expression = $"({expression})";
                    }
                    expression = $"{expression}/{scaled_row[pivot_column]}";
                }

                if (expression == string.Empty)
                {
                    expression = "0";
                }

                expressions.Add($"{Labeller(pivot_column)} = {expression}");
            }

            List<string> generalized_solution = new();
            generalized_solution.Add(GetEquationWithPlaceholders() + ", where");
            generalized_solution.AddRange(expressions);
            generalized_solution.Add("for any {" + string.Join(", ", free_var_indices.Select(Labeller)) + "}");

            Outcome = string.Join(Environment.NewLine, generalized_solution);
        }

        protected abstract CoreMatrix<T> GetReducedAugmentedMatrix();

        private string GetEquationWithPlaceholders()
        {
            List<string> l = new();
            List<string> r = new();

            for (int i = 0; i < fragments.Count; i++)
            {
                var t = Labeller(i) + MULTIPLICATION_SYMBOL + fragments[i];
                (i < ReactantsCount ? l : r).Add(t);
            }

            return string.Join(" + ", l) + " = " + string.Join(" + ", r);
        }
    }

    internal class RisteskiBalancer_Rational : RisteskiBalancer<Rational>
    {
        public RisteskiBalancer_Rational(string equation) : base(equation)
        {
        }

        protected override long[] VectorScaler(Rational[] v)
        {
            return Helpers.ScaleRationals(v);
        }

        protected override RationalMatrixInRREF GetReducedAugmentedMatrix()
        {
            var AM = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
            return new RationalMatrixInRREF(AM);
        }

        protected override string PrettyPrinter(Rational value)
        {
            return Helpers.PrettyPrintRational(value);
        }
    }

    internal class RisteskiBalancer_Double : RisteskiBalancer<double>
    {
        public RisteskiBalancer_Double(string equation) : base(equation)
        {
        }

        protected override long[] VectorScaler(double[] v)
        {
            return Helpers.ScaleDoubles(v);
        }

        protected override DoubleMatrixInRREF GetReducedAugmentedMatrix()
        {
            var AM = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
            return new DoubleMatrixInRREF(AM);
        }

        protected override string PrettyPrinter(double value)
        {
            return Helpers.PrettyPrintDouble(value);
        }
    }
}