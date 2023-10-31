namespace ReactionStoichiometry;

using System.Numerics;
using System.Text.RegularExpressions;
using Extensions;
using MathNet.Numerics.LinearAlgebra;

internal abstract partial class Balancer<T> : ISpecialToStringProvider, IChemicalEntityCollection
{
    private readonly String _skeletal;
    protected readonly List<String> Details = new();
    protected readonly List<String> Diagnostics = new();
    protected readonly List<String> Entities = new();
    protected readonly Matrix<Double> M;
    protected readonly Int32 ReactantsCount;
    protected abstract IEnumerable<String> Outcome { get; }

    protected String GetEquationWithPlaceholders
    {
        get
        {
            List<String> l = new();
            List<String> r = new();

            for (var i = 0; i < Entities.Count; i++)
            {
                var t = LabelFor(i) + Program.MULTIPLICATION_SYMBOL + Entities[i];
                (i < ReactantsCount ? l : r).Add(t);
            }

            return String.Join(" + ", l) + " = " + String.Join(" + ", r);
        }
    }

    protected Balancer(String equation)
    {
        try
        {
            _skeletal = equation.Replace(" ", "");
            ReactantsCount = _skeletal.Split('=')[0].Split('+').Length;
            Entities.AddRange(Regex.Split(_skeletal, Parsing.CRE_ALLOWED_DIVIDERS));

            var chargeSymbols = new[] { "Qn", "Qp" };
            var elements = Regex.Matches(_skeletal, Parsing.ELEMENT_SYMBOL).Select(static m => m.Value).Concat(chargeSymbols).Distinct().ToList();
            elements.Add("{e}");

            M = Matrix<Double>.Build.Dense(elements.Count, Entities.Count);
            for (var r = 0; r < elements.Count; r++)
            {
                Regex regex = new(Parsing.ELEMENT_TEMPLATE.Replace("X", elements[r]));

                for (var c = 0; c < Entities.Count; c++)
                {
                    var s = Parsing.Unfold(Entities[c]);
                    M[r, c] += regex.Matches(s).Sum(static match => Double.Parse(match.Groups[1].Value));
                }
            }

            var chargeParsingRules = new[] { new[] { "Qn", @"Qn(\d*)$", "{$1-}" }, new[] { "Qp", @"Qp(\d*)$", "{$1+}" } };
            foreach (var chargeParsingRule in chargeParsingRules)
            {
                for (var i = 0; i < Entities.Count; i++)
                {
                    Entities[i] = Regex.Replace(Entities[i], chargeParsingRule[1], chargeParsingRule[2]);
                }
            }

            var totalCharge = M.Row(elements.IndexOf("Qp")) - M.Row(elements.IndexOf("Qn"));
            M.SetRow(elements.IndexOf("{e}"), totalCharge);

            if (!totalCharge.Any(DoubleExtensions.IsNonZero))
            {
                M = M.RemoveRow(elements.IndexOf("{e}"));
                elements.Remove("{e}");
            }

            M = M.RemoveRow(elements.IndexOf("Qn"));
            elements.Remove("Qn");
            M = M.RemoveRow(elements.IndexOf("Qp"));
            elements.Remove("Qp");

            Details.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", M.ToArray(), Utils.PrettyPrintDouble, Entities, elements));
            Details.Add($"RxC: {M.RowCount}x{M.ColumnCount}, rank = {M.Rank()}, nullity = {M.Nullity()}");
        } catch (Exception e)
        {
            throw new BalancerException($"Parsing failed: {e.Message}");
        }

        try
        {
            Balance();
        } catch (BalancerException e)
        {
            Diagnostics.Add($"This equation can't be balanced: {e.Message}");
        }
    }

    public Int32 EntitiesCount => Entities.Count;
    public String Entity(Int32 i) => Entities[i];
    public String LabelFor(Int32 i) => Entities.Count > Program.LETTER_LABEL_THRESHOLD ? Utils.GenericLabel(i) : Utils.LetterLabel(i);

    public String ToString(ISpecialToStringProvider.OutputFormat format)
    {
        return format switch
        {
            ISpecialToStringProvider.OutputFormat.Plain => Fill(OutputTemplateStrings.PLAIN_OUTPUT),
            ISpecialToStringProvider.OutputFormat.Html => Fill(OutputTemplateStrings.HTML_OUTPUT),
            ISpecialToStringProvider.OutputFormat.OutcomeCommaSeparated => String.Join(',', Outcome),
            ISpecialToStringProvider.OutputFormat.OutcomeNewLineSeparated => String.Join(Environment.NewLine, Outcome),
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };

        String Fill(String template)
        {
            return template.Replace("%Skeletal%", _skeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", ToString(ISpecialToStringProvider.OutputFormat.OutcomeNewLineSeparated))
                           .Replace("%Diagnostics%", String.Join(Environment.NewLine, Diagnostics));
        }
    }
    
    protected String GetEquationWithCoefficients(in BigInteger[] coefficients)
    {
        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < Entities.Count; i++)
        {
            if (coefficients[i] == 0) continue;

            var value = BigInteger.Abs(coefficients[i]);
            var t = (value == 1 ? "" : value + Program.MULTIPLICATION_SYMBOL) + Entities[i];
            (coefficients[i] < 0 ? l : r).Add(t);
        }

        if (l.Count == 0 || r.Count == 0) return "Invalid coefficients";
        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    protected abstract void Balance();
    protected abstract BigInteger[] ScaleToIntegers(T[] v);
    protected abstract String PrettyPrinter(T value);
}