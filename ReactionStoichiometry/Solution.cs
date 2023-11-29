using Newtonsoft.Json;

namespace ReactionStoichiometry
{
    public abstract class Solution
    {
        [JsonIgnore]
        private protected readonly ChemicalReactionEquation Equation;

        [JsonProperty(PropertyName = "Success")]
        public Boolean Success { get; private protected init; }
        protected String? AsMultilineString { get; private protected init; }
        protected String? AsSimpleString { get; private protected init; }

        [JsonProperty(PropertyName = "FailureMessage")]
        protected String? FailureMessage { get; private protected init; }

        protected Solution(ChemicalReactionEquation equation)
        {
            Equation = equation;
            CheckPrerequisites();
            return;

            void CheckPrerequisites()
            {
                AppSpecificException.ThrowIf(
                    !Enumerable.Range(Equation.RREF.ColumnCount() - Equation.CompositionMatrixNullity, Equation.CompositionMatrixNullity)
                               .SequenceEqual(Equation.SpecialColumnsOfRREF ?? throw new InvalidOperationException())
                  , String.Format(format: "Free variables are misplaced, {0} must be the rightmost"
                                , String.Join(separator: " and ", Equation.SpecialColumnsOfRREF.Select(selector: i => Equation.Substances[i]))));
            }
        }

        public String ToString(OutputFormat format)
        {
            return format switch
                   {
                       OutputFormat.Simple => AsSimpleString
                     , OutputFormat.Multiline => AsMultilineString
                     , OutputFormat.DetailedMultiline => AsDetailedMultilineString()
                     , _ => throw new ArgumentOutOfRangeException(nameof(format))
                   }
                ?? throw new NullReferenceException();

            String AsDetailedMultilineString()
            {
                var result = OutputFormatTemplates.MULTILINE_TEMPLATE.Replace(oldValue: "%Skeletal%", Equation.OriginalString)
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
}
