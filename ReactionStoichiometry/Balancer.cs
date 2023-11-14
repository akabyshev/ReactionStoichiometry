using System.Numerics;
using Rationals;


namespace ReactionStoichiometry
{
    public abstract class Balancer
    {
        #region OutputFormat enum
        public enum OutputFormat
        {
            Simple
          , SeparateLines
          , DetailedPlain
          , DetailedHtml
        }
        #endregion

        public readonly ChemicalReactionEquation Equation;

        private readonly List<String> _details = new();
        private String _failureMessage = String.Empty;

        protected Balancer(String equation)
        {
            Equation = new ChemicalReactionEquation(equation.Replace(oldValue: " ", String.Empty));
        }

        protected abstract void Balance();

        public virtual String ToString(OutputFormat format)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return format switch
            {
                OutputFormat.DetailedPlain => Fill(OutputFormatTemplates.PLAIN_OUTPUT)
              , OutputFormat.DetailedHtml => Fill(OutputFormatTemplates.HTML_OUTPUT)
              , _ => throw new ArgumentOutOfRangeException(nameof(format))
            };

            String Fill(String template)
            {
                return template.Replace(oldValue: "%Skeletal%", Equation.Skeletal)
                               .Replace(oldValue: "%Details%", String.Join(Environment.NewLine, _details))
                               .Replace(oldValue: "%Outcome%", ToString(OutputFormat.SeparateLines))
                               .Replace(oldValue: "%Diagnostics%", _failureMessage);
            }
        }

        public Boolean Run()
        {
            _details.Add(Equation.CCM.Readable(title: "Chemical composition matrix", columnHeaders: i => Equation.Substances[i]));
            _details.Add(String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                     , Equation.CCM.RowCount()
                                     , Equation.CCM.ColumnCount()
                                     , Equation.CompositionMatrixRank
                                     , Equation.CompositionMatrixNullity));
            _details.Add(Equation.RREF.Readable(title: "RREF", LabelFor, LabelFor));

            try
            {
                Balance();
            }
            catch (AppSpecificException e)
            {
                _failureMessage = "This equation can't be balanced: " + e.Message;
                return false;
            }

            return true;
        }

        internal Boolean ValidateSolution(BigInteger[] coefficients)
        {
            if (coefficients.Length != Equation.Substances.Count)
            {
                throw new ArgumentException(message: "Size mismatch");
            }

            for (var r = 0; r < Equation.CCM.RowCount(); r++)
            {
                var sum = Rational.Zero;
                for (var c = 0; c < Equation.CCM.ColumnCount(); c++)
                {
                    sum += Equation.CCM[r, c] * coefficients[c];
                }
                if (sum != Rational.Zero)
                {
                    return false;
                }
            }

            return true;
        } // ReSharper disable twice ArgumentsStyleNamedExpression
        public String EquationWithPlaceholders()
        {
            return StringOperations.AssembleEquationString(strings: Equation.Substances
                                                         , Enumerable.Range(start: 0, Equation.Substances.Count).ToArray()
                                                         , omit: static _ => false
                                                         , adapter: LabelFor
                                                         , predicateGoesToRHS: static _ => false
                                                         , allowEmptyRHS: true);
        }

        public String EquationWithIntegerCoefficients(BigInteger[] coefficients)
        {
            return StringOperations.AssembleEquationString(strings: Equation.Substances
                                                         , coefficients
                                                         , omit: static value => value.IsZero
                                                         , adapter: static value =>
                                                                        BigInteger.Abs(value) == 1 ? String.Empty : BigInteger.Abs(value).ToString()
                                                         , predicateGoesToRHS: static value => value > 0);
        }

        public String LabelFor(Int32 i)
        {
            return Equation.Substances.Count > GlobalConstants.LETTER_LABEL_THRESHOLD ?
                'x' + (i + 1).ToString(format: "D2") :
                ((Char)('a' + i)).ToString();
        }
    }
}
