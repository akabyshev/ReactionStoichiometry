﻿using System.Numerics;
using System.Text.RegularExpressions;

namespace ReactionStoichiometry
{
    internal static class StringOperations
    {
        internal const String ELEMENT_SYMBOL = "[A-Z][a-z]|[A-Z]";
        internal const String ELEMENT_TEMPLATE = @"X(\d+(?:\.\d+)*)";
        private const String CLOSING_PARENTHESIS = @"\)";

        private const String INNERMOST_PARENTHESES_WITH_INDEX =
            @$"{OPENING_PARENTHESIS}([^{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+){CLOSING_PARENTHESIS}(\d+)";

        private const String NO_INDEX_CLOSING_PARENTHESIS = @$"{CLOSING_PARENTHESIS}(?!\d)";
        private const String NO_INDEX_ELEMENT = $"({ELEMENT_SYMBOL})({ELEMENT_SYMBOL}|{OPENING_PARENTHESIS}|{CLOSING_PARENTHESIS}|$)";
        private const String OPENING_PARENTHESIS = @"\(";

        private const String SKELETAL_STRUCTURE = @$"^(?:{SUBSTANCE_ALPHABET}\+)*{SUBSTANCE_ALPHABET}={SUBSTANCE_ALPHABET}(?:\+{SUBSTANCE_ALPHABET})*$";

        private const String SUBSTANCE_ALPHABET = @$"[A-Za-z0-9\.{OPENING_PARENTHESIS}{CLOSING_PARENTHESIS}]+";

        internal static Boolean LooksLikeChemicalReactionEquation(String equationString)
        {
            return Regex.IsMatch(equationString, SKELETAL_STRUCTURE);
        }

        internal static String UnfoldSubstance(String substance)
        {
            var result = substance;

            {
                Regex regex = new(NO_INDEX_ELEMENT);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success)
                    {
                        break;
                    }
                    var element = match.Groups[groupnum: 1].Value;
                    var rest = match.Groups[groupnum: 2].Value;
                    result = regex.Replace(result, element + "1" + rest, count: 1);
                }
            }
            {
                Regex regex = new(NO_INDEX_CLOSING_PARENTHESIS);
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
                Regex regex = new(INNERMOST_PARENTHESES_WITH_INDEX);
                while (true)
                {
                    var match = regex.Match(result);
                    if (!match.Success)
                    {
                        break;
                    }
                    var token = match.Groups[groupnum: 1].Value;
                    var index = match.Groups[groupnum: 2].Value;

                    var repeated = String.Join(String.Empty, Enumerable.Repeat(token, Int32.Parse(index)));
                    result = regex.Replace(result, repeated, count: 1);
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

        internal static BigInteger[] GetParametersFromString(String s)
        {
            return s.Trim('(', ')').Split(separator: ',').Select(BigInteger.Parse).ToArray();
        }
    }
}
