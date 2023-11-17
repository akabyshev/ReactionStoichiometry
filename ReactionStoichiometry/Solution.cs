using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public abstract class Solution
    {
        private const String UNINITIALIZED_STRING = "UNINIT";
        [JsonProperty(PropertyName = "Type")]
        internal abstract String Name { get; }

        [JsonProperty(PropertyName = "Failure message")]
        private String _failureMessage = String.Empty;

        [field: JsonProperty(PropertyName = "Success")]
        public Boolean Success { get; private set; }

        private Boolean _hadBeenRunAlready;

        // ReSharper disable once MemberCanBePrivate.Global
        private protected String AsDetailedMultilineString = UNINITIALIZED_STRING;
        private protected String AsSimpleString = UNINITIALIZED_STRING;
        private protected String AsMultilineString = UNINITIALIZED_STRING;

        protected abstract void WorkOn_Impl(ChemicalReactionEquation equation);

        internal Boolean WorkOn(ChemicalReactionEquation equation)
        {
            AppSpecificException.ThrowIf(_hadBeenRunAlready, message: "Invalid call");

            _hadBeenRunAlready = true;
            try
            {
                WorkOn_Impl(equation);
                AsDetailedMultilineString = OutputFormatTemplates.MULTILINE_TEMPLATE.Replace(oldValue: "%Skeletal%", equation.OriginalEquation)
                                                                 .Replace(oldValue: "%CCM%"
                                                                        , equation.CCM.Readable(title: "CCM", columnHeaders: i => equation.Substances[i]))
                                                                 .Replace(oldValue: "%RREF%"
                                                                        , equation.RREF.Readable(title: "RREF", equation.LabelFor, equation.LabelFor))
                                                                 .Replace(oldValue: "%CCM_stats%"
                                                                        , String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                                                                      , equation.CCM.RowCount()
                                                                                      , equation.CCM.ColumnCount()
                                                                                      , equation.CompositionMatrixRank
                                                                                      , equation.CompositionMatrixNullity))
                                                                 .Replace(oldValue: "%Outcome%", AsMultilineString)
                                                                 .Replace(oldValue: "%Diagnostics%", _failureMessage);
                Success = true;
            }
            catch (AppSpecificException e)
            {
                _failureMessage = "This equation can't be balanced: " + e.Message;
                Success = false;
            }

            return Success;
        }

        public abstract String ToString(OutputFormat format);
    }
}
