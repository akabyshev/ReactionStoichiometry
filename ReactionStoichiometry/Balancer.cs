using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public abstract class Balancer
    {
        [JsonProperty]
        internal readonly ChemicalReactionEquation Equation;

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
        }

        protected abstract void BalanceImplementation();

        public virtual String ToString(OutputFormat format)
        {
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return format switch
            {
                OutputFormat.DetailedMultiline => FillDetailedMultilineTemplate()
              , OutputFormat.Json => SerializeToJson()
              , _ => throw new ArgumentOutOfRangeException(nameof(format))
            };

            String FillDetailedMultilineTemplate()
            {
                return OutputFormatTemplates.MULTILINE_TEMPLATE.Replace(oldValue: "%Skeletal%", Equation.OriginalEquation)
                                            .Replace(oldValue: "%CCM%", Equation.CCM.Readable(title: "CCM", columnHeaders: i => Equation.Substances[i]))
                                            .Replace(oldValue: "%RREF%", Equation.RREF.Readable(title: "RREF", Equation.LabelFor, Equation.LabelFor))
                                            .Replace(oldValue: "%CCM_stats%"
                                                   , String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                                                 , Equation.CCM.RowCount()
                                                                 , Equation.CCM.ColumnCount()
                                                                 , Equation.CompositionMatrixRank
                                                                 , Equation.CompositionMatrixNullity))
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
