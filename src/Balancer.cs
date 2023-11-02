namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal abstract class Balancer<T> : ISpecialToStringProvider, IChemicalEntityList
{
    private readonly String _failureMessage;
    protected readonly List<String> Details = new();
    protected readonly ChemicalReactionEquation Equation;
    protected readonly Matrix<Double> M;
    protected readonly Func<T, String> PrettyPrinter;
    protected readonly Func<T[], BigInteger[]> ScaleToIntegers;

    protected Balancer(String equation, Func<T, String> print, Func<T[], BigInteger[]> scale)
    {
        Equation = new ChemicalReactionEquation(equation);
        PrettyPrinter = print;
        ScaleToIntegers = scale;

        try
        {
            Equation.Parse(out M);
        }
        catch (Exception e)
        {
            throw new BalancerException($"Parsing failed: {e.Message}");
        }

        Details.AddRange(Utils.PrettyPrintMatrix("Chemical composition matrix", M.ToArray(), Utils.PrettyPrintDouble, GetEntity, Equation.GetElement));
        Details.Add($"RxC: {M.RowCount}x{M.ColumnCount}, rank = {M.Rank()}, nullity = {M.Nullity()}");

        try
        {
            Balance();
            _failureMessage = String.Empty;
        }
        catch (BalancerException e)
        {
            _failureMessage = "This equation can't be balanced: " + e.Message;
        }
    }

    protected abstract IEnumerable<String> Outcome { get; }

    #region IChemicalEntityList Members
    public Int32 EntitiesCount => Equation.EntitiesCount;
    public String GetEntity(Int32 i) => Equation.GetEntity(i);
    #endregion

    #region ISpecialToStringProvider Members
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
            return template.Replace("%Skeletal%", Equation.Skeletal)
                           .Replace("%Details%", String.Join(Environment.NewLine, Details))
                           .Replace("%Outcome%", ToString(ISpecialToStringProvider.OutputFormat.OutcomeNewLineSeparated))
                           .Replace("%Diagnostics%", _failureMessage);
        }
    }
    #endregion

    protected String AssembleEquationString<T2>(T2[] vector, Func<T2, Boolean> mustInclude, Func<T2, String> toString, Func<Int32, T2, Boolean> isReactant)
    {
        if (vector.Length != EntitiesCount) throw new ArgumentOutOfRangeException(nameof(vector), "Array size mismatch");

        List<String> l = new();
        List<String> r = new();

        for (var i = 0; i < EntitiesCount; i++)
        {
            if (mustInclude(vector[i])) (isReactant(i, vector[i]) ? l : r).Add(toString(vector[i]) + GetEntity(i));
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

    #region Nested type: BalancerException
    internal sealed class BalancerException : InvalidOperationException
    {
        public BalancerException(String message) : base(message)
        {
        }
    }
    #endregion
}