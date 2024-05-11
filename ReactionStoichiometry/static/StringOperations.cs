using System.Text.RegularExpressions;

namespace ReactionStoichiometry
{
    internal static class StringOperations
    {
        internal const String ELEMENT_SYMBOL = "[A-Z][a-z]|[A-Z]";
        internal const String INDEX = @"(\d+(?:\.\d+)*)";
        private const String ELEMENT_WITH_NO_INDEX = $"({ELEMENT_SYMBOL})(?={ELEMENT_SYMBOL}|{PARENTHESIS_OPENING}|{PARENTHESIS_CLOSING}|$)";

        private const String EQUATION_STRUCTURE = @$"^(?:{SUBSTANCE}\+)*{SUBSTANCE}={SUBSTANCE}(?:\+{SUBSTANCE})*$";

        private const String PARENTHESES_INNERMOST_PAIR_WITH_INDEX =
            $"{PARENTHESIS_OPENING}([^{PARENTHESIS_OPENING}{PARENTHESIS_CLOSING}]+){PARENTHESIS_CLOSING}({INDEX})";

        private const String PARENTHESIS_CLOSING = @"\)";
        private const String PARENTHESIS_CLOSING_WITH_NO_INDEX = $"{PARENTHESIS_CLOSING}(?!{INDEX})";
        private const String PARENTHESIS_OPENING = @"\(";

        private const String SUBSTANCE = @$"[A-Za-z0-9\.{PARENTHESIS_OPENING}{PARENTHESIS_CLOSING}]+";

        internal static Boolean LooksLikeChemicalReactionEquation(String equationString)
        {
            return Regex.IsMatch(equationString, EQUATION_STRUCTURE);
        }

        internal static String UnfoldSubstance(String substance)
        {
            var result = substance;

            {
                Regex regex = new(ELEMENT_WITH_NO_INDEX);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success)
                    {
                        break;
                    }
                    var element = match.Groups[groupnum: 1].Value;
                    result = regex.Replace(result, element + "1", count: 1);
                }
            }
            {
                Regex regex = new(PARENTHESIS_CLOSING_WITH_NO_INDEX);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success)
                    {
                        break;
                    }

                    result = regex.Replace(result, replacement: ")1", count: 1);
                }
            }
            {
                Regex regex = new(PARENTHESES_INNERMOST_PAIR_WITH_INDEX);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success)
                    {
                        break;
                    }
                    var token = match.Groups[groupnum: 1].Value;
                    var index = match.Groups[groupnum: 2].Value;
                    result = regex.Replace(result, String.Join(String.Empty, Enumerable.Repeat(token, Int32.Parse(index))), count: 1);
                }
            }

            return result;
        }

        internal static String AssembleEquationString<T>(IReadOnlyList<String> strings
                                                       , IReadOnlyList<T> values
                                                       , Func<T, Boolean> omitIf
                                                       , Func<T, String> adapter
                                                       , Func<T, Boolean> goesToRhsIf
                                                       , Boolean allowEmptyRhs)
        {
            List<String> lhs = new();
            List<String> rhs = new();

            for (var i = 0; i < values.Count; i++)
            {
                if (omitIf(values[i]))
                {
                    continue;
                }

                var token = adapter(values[i]);
                if (token != String.Empty)
                {
                    token += GlobalConstants.MULTIPLICATION_SYMBOL;
                }

                (goesToRhsIf(values[i]) ? rhs : lhs).Add(token + strings[i]);
            }

            if (rhs.Count == 0 && allowEmptyRhs)
            {
                rhs.Add(item: "0");
            }

            AppSpecificException.ThrowIf(lhs.Count == 0 || rhs.Count == 0, message: "Both LHS and RHS lists must have elements");

            return String.Join(separator: " + ", lhs) + " = " + String.Join(separator: " + ", rhs);
        }

        internal static String CoefficientsAsString<T>(this IEnumerable<T> me)
        {
            return "{" + String.Join(separator: ", ", me) + "}";
        }
    }
}
