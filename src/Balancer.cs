namespace ReactionStoichiometry;

using System.Numerics;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

internal abstract partial class Balancer<T> : ISpecialToStringProvider, IChemicalEntityCollection
{
    private readonly List<String> _entities = new();
    private readonly String _failureMessage;
    private readonly String _skeletal;
    protected readonly List<String> Details = new();
    protected readonly Matrix<Double> M;
    protected readonly Func<T, String> PrettyPrinter;
    protected readonly Int32 ReactantsCount;
    protected readonly Func<T[], BigInteger[]> ScaleToIntegers;
    protected abstract IEnumerable<String> Outcome { get; }

    protected Balancer(String equation, Func<T, String> print, Func<T[], BigInteger[]> scale)
    {
        _skeletal = equation.Replace(" ", "");
        PrettyPrinter = print;
        ScaleToIntegers = scale;

        try
        {
            ReactantsCount = _skeletal.Split('=')[0].Split('+').Length;
            _entities.AddRange(Regex.Split(_skeletal, Parsing.DIVIDER_CHARS));

            var chargeSymbols = new[] { "Qn", "Qp" };
            var elements = Regex.Matches(_skeletal, Parsing.ELEMENT_SYMBOL).Select(static m => m.Value).Concat(chargeSymbols).Distinct().ToList();
            elements.Add("{e}");

            M = Matrix<Double>.Build.Dense(elements.Count, _entities.Count);
            for (var r = 0; r < elements.Count; r++)
            {
                Regex regex = new(Parsing.ELEMENT_TEMPLATE.Replace("X", elements[r]));

                for (var c = 0; c < _entities.Count; c++)
                {
                    var s = Parsing.Unfold(_entities[c]);
                    M[r, c] += regex.Matches(s).Sum(static match => Double.Parse(match.Groups[1].Value));
                }
            }

            var chargeParsingRules = new[] { new[] { "Qn", @"Qn(\d*)$", "{$1-}" }, new[] { "Qp", @"Qp(\d*)$", "{$1+}" } };
            foreach (var chargeParsingRule in chargeParsingRules)
            {
                for (var i = 0; i < _entities.Count; i++)
                {
                    _entities[i] = Regex.Replace(_entities[i], chargeParsingRule[1], chargeParsingRule[2]);
                }
            }

            var totalCharge = M.Row(elements.IndexOf("Qp")) - M.Row(elements.IndexOf("Qn"));
            M.SetRow(elements.IndexOf("{e}"), totalCharge);

            if (!totalCharge.Any(Utils.IsNonZeroDouble))
            {
                M = M.RemoveRow(elements.IndexOf("{e}"));
                elements.Remove("{e}");
            }

            M = M.RemoveRow(elements.IndexOf("Qn"));
            elements.Remove("Qn");
            M = M.RemoveRow(elements.IndexOf("Qp"));
            elements.Remove("Qp");

            Details.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", M.ToArray(), Utils.PrettyPrintDouble, _entities, elements));
            Details.Add($"RxC: {M.RowCount}x{M.ColumnCount}, rank = {M.Rank()}, nullity = {M.Nullity()}");
        } catch (Exception e)
        {
            throw new BalancerException($"Parsing failed: {e.Message}");
        }

        try
        {
            Balance();
            _failureMessage = String.Empty;
        } catch (BalancerException e)
        {
            _failureMessage = "This equation can't be balanced: " + e.Message;
        }
    }

    public Int32 EntitiesCount => _entities.Count;
    public String Entity(Int32 i) => _entities[i];

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
                           .Replace("%Diagnostics%", _failureMessage);
        }
    }

    protected String AssembleEquationString<T2>(T2[] vector, Func<T2, Boolean> mustInclude, Func<T2, String> toString, Func<Int32, T2, Boolean> isReactant)
    {
        if (vector.Length != EntitiesCount) throw new ArgumentOutOfRangeException(nameof(vector), "Array size mismatch");

        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < EntitiesCount; i++)
        {
            if (mustInclude(vector[i])) (isReactant(i, vector[i]) ? l : r).Add(toString(vector[i]) + Entity(i));
        }

        if (l.Count == 0 || r.Count == 0) return "Invalid coefficients";
        return String.Join(" + ", l) + " = " + String.Join(" + ", r);
    }

    protected String EquationWithIntegerCoefficients(BigInteger[] coefficients) =>
        AssembleEquationString(coefficients,
                               static value => value != BigInteger.Zero,
                               static value => value == 1 || value == -1 ? "" : BigInteger.Abs(value) + Program.MULTIPLICATION_SYMBOL,
                               static (_, value) => value < 0);

    protected abstract void Balance();
}