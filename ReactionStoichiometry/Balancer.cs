using System.Numerics;
using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry
{
    public abstract class Balancer
    {
        #region OutputFormat enum
        public enum OutputFormat
        {
            Simple
          , Multiline
          , DetailedMultiline
          , Json
        }
        #endregion

        public readonly ChemicalReactionEquation Equation;
        private readonly List<String> _details = new();

        [JsonProperty(PropertyName = "Failure message")]
        private String _failureMessage = String.Empty;

        private Boolean _hadBeenRunAlready;

        [JsonProperty(PropertyName = "Success")]
        private Boolean _success;

        protected Balancer(String equation)
        {
            Equation = new ChemicalReactionEquation(equation.Replace(oldValue: " ", String.Empty));
            _details.Add(Equation.CCM.Readable(title: "CCM", columnHeaders: i => Equation.Substances[i]));
            _details.Add(String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                     , Equation.CCM.RowCount()
                                     , Equation.CCM.ColumnCount()
                                     , Equation.CompositionMatrixRank
                                     , Equation.CompositionMatrixNullity));
            _details.Add(Equation.RREF.Readable(title: "RREF", Equation.LabelFor, Equation.LabelFor));
        }

        protected abstract void Balance();

        public virtual String ToString(OutputFormat format)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return format switch
            {
                OutputFormat.DetailedMultiline => FillTemplate(OutputFormatTemplates.MULTILINE_TEMPLATE)
              , OutputFormat.Json => SerializeToJson()
              , _ => throw new ArgumentOutOfRangeException(nameof(format))
            };

            String FillTemplate(String template)
            {
                return template.Replace(oldValue: "%Skeletal%", Equation.Skeletal)
                               .Replace(oldValue: "%Details%", String.Join(Environment.NewLine, _details))
                               .Replace(oldValue: "%Outcome%", ToString(OutputFormat.Multiline))
                               .Replace(oldValue: "%Diagnostics%", _failureMessage);
            }
        }

        public Boolean Run()
        {
            AppSpecificException.ThrowIf(_hadBeenRunAlready, message: "Invalid call");

            _hadBeenRunAlready = true;
            try
            {
                Balance();
                _success = true;
            }
            catch (AppSpecificException e)
            {
                _failureMessage = "This equation can't be balanced: " + e.Message;
                _success = false;
            }

            return _success;
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
        }

        private String SerializeToJson()
        {
            var settings = new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                             , Formatting = Formatting.Indented
                             , ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                           };

            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
