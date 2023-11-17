using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public abstract class Balancer
    {
        [JsonProperty]
        internal readonly ChemicalReactionEquation Equation;

        private readonly List<String> _details = new();

        [JsonProperty(PropertyName = "Failure message")]
        private String _failureMessage = String.Empty;

        private Boolean _hadBeenRunAlready;

        [JsonProperty(PropertyName = "Success")]
        private Boolean _success;

        protected Balancer(String equationString) : this(new ChemicalReactionEquation(equationString))
        {
        }

        protected Balancer(ChemicalReactionEquation equation)
        {
            Equation = equation;
            _details.Add(Equation.CCM.Readable(title: "CCM", columnHeaders: i => Equation.Substances[i]));
            _details.Add(String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                     , Equation.CCM.RowCount()
                                     , Equation.CCM.ColumnCount()
                                     , Equation.CompositionMatrixRank
                                     , Equation.CompositionMatrixNullity));
            _details.Add(Equation.RREF.Readable(title: "RREF", Equation.LabelFor, Equation.LabelFor));
        }

        protected abstract void BalanceImplementation();

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
                return template.Replace(oldValue: "%Skeletal%", Equation.OriginalEquation)
                               .Replace(oldValue: "%Details%", String.Join(Environment.NewLine, _details))
                               .Replace(oldValue: "%Outcome%", ToString(OutputFormat.Multiline))
                               .Replace(oldValue: "%Diagnostics%", _failureMessage);
            }
        }

        public Boolean Balance()
        {
            AppSpecificException.ThrowIf(_hadBeenRunAlready, message: "Invalid call");

            _hadBeenRunAlready = true;
            try
            {
                BalanceImplementation();
                _success = true;
            }
            catch (AppSpecificException e)
            {
                _failureMessage = "This equation can't be balanced: " + e.Message;
                _success = false;
            }

            return _success;
        }

        private String SerializeToJson()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            return JsonConvert.SerializeObject(this, settings);
        }
    }
}
