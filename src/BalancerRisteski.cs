namespace ReactionStoichiometry;

using System.Numerics;
using Properties;

public sealed class BalancerRisteski : Balancer
{
    private Dictionary<Int32, BigInteger[]>? _dependentCoefficientExpressions;
    private List<Int32>? _freeCoefficientIndices;

    internal Int32 DegreesOfFreedom => _freeCoefficientIndices!.Count;

    protected override IEnumerable<String> Outcome
    {
        get
        {
            if (_dependentCoefficientExpressions == null || _freeCoefficientIndices == null) return new[] { "<FAIL>" };

            List<String> lines = new() { EquationWithPlaceholders() + ", where" };
            lines.AddRange(_dependentCoefficientExpressions.Keys.Select(selector: i => $"{LabelFor(i)} = {AlgebraicExpressionForCoefficient(i)}"));
            lines.Add("for any {" + String.Join(separator: ", ", _freeCoefficientIndices.Select(LabelFor)) + "}");

            return lines;
        }
    }

    public BalancerRisteski(String equation) : base(equation)
    {
    }

    public override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.VectorsNotation) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_dependentCoefficientExpressions == null || _freeCoefficientIndices == null) return "<FAIL>";

        return DegreesOfFreedom
             + ":{"
             + String.Join(separator: ", "
                         , Enumerable.Range(start: 0, Equation.SubstancesCount)
                                     .Select(selector: i => _freeCoefficientIndices.Contains(i) ? LabelFor(i) : AlgebraicExpressionForCoefficient(i)))
             + '}';
    }

    protected override void BalanceImplementation()
    {
        BalancerException.ThrowIf(Equation.CompositionMatrixReduced.IsIdentityMatrix(), message: "SLE is unsolvable");

        Details.AddRange(Equation.CompositionMatrixReduced.ToString(title: "Reduced matrix"));

        _dependentCoefficientExpressions = Enumerable.Range(start: 0, Equation.CompositionMatrixReduced.RowCount())
                                                     .Select(selector: r => Equation.CompositionMatrixReduced.Row(r)
                                                                                    .ScaleToIntegers()
                                                                                    .Select(selector: static i => -i)
                                                                                    .ToArray())
                                                     .ToDictionary(keySelector: static row => Array.FindIndex(row, match: static i => i != 0)
                                                                 , elementSelector: static row => row);

        _freeCoefficientIndices = Enumerable.Range(start: 0, Equation.CompositionMatrixReduced.ColumnCount())
                                            .Where(predicate: c => !_dependentCoefficientExpressions.ContainsKey(c)
                                                                && Equation.CompositionMatrixReduced.Column(c).Any(static t => t != 0))
                                            .ToList();
    }

    public String? AlgebraicExpressionForCoefficient(Int32 index)
    {
        // TODO: this has a lot of common logic with EquationWithIntegerCoefficients that calls AssembleEquationString<T>, but the output is much better in context. Try to generalize?
        // "a = c/2" vs "2a = c" thing

        if (_dependentCoefficientExpressions == null) throw new InvalidOperationException();

        if (!_dependentCoefficientExpressions.ContainsKey(index)) return null;

        var expression = _dependentCoefficientExpressions[index];

        var numeratorParts = Enumerable.Range(index + 1, expression.Length - (index + 1))
                                       .Where(predicate: i => expression[i] != 0)
                                       .Select(selector: i =>
                                                         {
                                                             var coefficient = expression[i] + Settings.Default.MULTIPLICATION_SYMBOL;
                                                             if (expression[i] == 1) coefficient = String.Empty;
                                                             if (expression[i] == -1) coefficient = "-";
                                                             return $"{coefficient}{LabelFor(i)}";
                                                         })
                                       .ToList();

        var result = String.Join(separator: " + ", numeratorParts).Replace(oldValue: "+ -", newValue: "- ");

        if (expression[index] != -1)
        {
            if (numeratorParts.Count > 1) result = $"({result})";
            result = $"{result}/{BigInteger.Abs(expression[index])}";
        }

        if (result == String.Empty) result = "0";

        return result;
    }

    public (BigInteger[] coefficients, String readable) Instantiate(BigInteger[] parameters)
    {
        if (parameters.Length != _freeCoefficientIndices!.Count)
            throw new ArgumentOutOfRangeException(nameof(parameters), message: "Array size mismatch");

        var result = new BigInteger[SubstancesCount];

        for (var i = 0; i < _freeCoefficientIndices.Count; i++)
        {
            result[_freeCoefficientIndices[i]] = parameters[i];
        }

        foreach (var kvp in _dependentCoefficientExpressions!)
        {
            var calculated = _freeCoefficientIndices.Aggregate(BigInteger.Zero, func: (sum, i) => sum + result[i] * kvp.Value[i]);
            BalancerException.ThrowIf(calculated % kvp.Value[kvp.Key] != 0, message: "Non-integer coefficient, try other SLE params");
            calculated /= kvp.Value[kvp.Key];

            result[kvp.Key] = -calculated;
        }

        return (result, EquationWithIntegerCoefficients(result));
    }
}
