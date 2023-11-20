using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public abstract class Solution
    {
        private const String UNINITIALIZED_STRING = "UNINIT";

        private protected String AsDetailedMultilineString = UNINITIALIZED_STRING;
        private protected String AsMultilineString = UNINITIALIZED_STRING;
        private protected String AsSimpleString = UNINITIALIZED_STRING;

        [JsonIgnore]
        private protected readonly ChemicalReactionEquation Equation;

        [JsonProperty(PropertyName = "Failure message")]
        private protected String? FailureMessage;

        [field: JsonProperty(PropertyName = "Success")]
        public Boolean Success { get; private protected init; }

        protected Solution(ChemicalReactionEquation equation)
        {
            Equation = equation;
        }

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

        private protected String GetAsDetailedMultilineString()
        {
            var result = OutputFormatTemplates.MULTILINE_TEMPLATE.Replace(oldValue: "%Skeletal%", Equation.InOriginalForm)
                                              .Replace(oldValue: "%CCM%"
                                                     , Equation.CCM.Readable(title: "CCM"
                                                                           , rowHeaders: i => Equation.ChemicalElements[i]
                                                                           , columnHeaders: i => Equation.Substances[i]))
                                              .Replace(oldValue: "%RREF%"
                                                     , Equation.RREF.Readable(title: "RREF"
                                                                            , rowHeaders: i => Equation.Labels[i]
                                                                            , columnHeaders: i => Equation.Labels[i]))
                                              .Replace(oldValue: "%CCM_stats%"
                                                     , String.Format(format: "RxC: {0}x{1}, rank = {2}, nullity = {3}"
                                                                   , Equation.CCM.RowCount()
                                                                   , Equation.CCM.ColumnCount()
                                                                   , Equation.CompositionMatrixRank
                                                                   , Equation.CompositionMatrixNullity));
            return result.Replace(oldValue: "%Outcome%", Success ? AsMultilineString : AsMultilineString + Environment.NewLine + FailureMessage);
        }
    }
}
