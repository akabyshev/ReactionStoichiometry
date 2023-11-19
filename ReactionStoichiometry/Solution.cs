using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public abstract class Solution
    {
        private const String UNINITIALIZED_STRING = "UNINIT";

        private protected String AsDetailedMultilineString = UNINITIALIZED_STRING;
        private protected String AsMultilineString = UNINITIALIZED_STRING;
        private protected String AsSimpleString = UNINITIALIZED_STRING;

        [JsonProperty(PropertyName = "Failure message")]
        private protected String? FailureMessage;

        [field: JsonProperty(PropertyName = "Success")]
        public Boolean Success { get; private protected init; }

        public String ToString(OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Simple => AsSimpleString
              , OutputFormat.Multiline => AsMultilineString
              , OutputFormat.DetailedMultiline => AsDetailedMultilineString
              , _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }

        private protected String GetAsDetailedMultilineString(ChemicalReactionEquation equation)
        {
            var result = OutputFormatTemplates.MULTILINE_TEMPLATE.Replace(oldValue: "%Skeletal%", equation.OriginalEquation)
                                              .Replace(oldValue: "%CCM%"
                                                     , equation.CCM.Readable(title: "CCM"
                                                                           , rowHeaders: i => equation.Elements[i]
                                                                           , columnHeaders: i => equation.Substances[i]))
                                              .Replace(oldValue: "%RREF%"
                                                     , equation.RREF.Readable(title: "RREF"
                                                                            , rowHeaders: i => equation.Labels[i]
                                                                            , columnHeaders: i => equation.Labels[i]))
                                              .Replace(oldValue: "%CCM_stats%"
                                                     , String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                                                   , equation.CCM.RowCount()
                                                                   , equation.CCM.ColumnCount()
                                                                   , equation.CompositionMatrixRank
                                                                   , equation.CompositionMatrixNullity));
            return result.Replace(oldValue: "%Outcome%", Success ? AsMultilineString : AsMultilineString + Environment.NewLine + FailureMessage);
        }
    }
}
