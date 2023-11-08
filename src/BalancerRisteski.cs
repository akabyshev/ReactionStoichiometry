namespace ReactionStoichiometry;

using System.Numerics;
using Properties;

internal class BalancerRisteski : Balancer
{
    private Dictionary<Int32, BigInteger[]>? _dependentCoefficientExpressions;
    private List<Int32>? _freeCoefficientIndices;

    public BalancerRisteski(String equation) : base(equation)
    {
    }

    internal Int32 DegreesOfFreedom => _freeCoefficientIndices!.Count;

    // implementations of Outcome share logic with ToString(). gotta do something
    protected override IEnumerable<String> Outcome
    {
        get
        {
            if (_dependentCoefficientExpressions == null || _freeCoefficientIndices == null) return new[] { "<FAIL>" };

            List<String> lines = new() { EquationWithPlaceholders() + ", where" };
            lines.AddRange(_dependentCoefficientExpressions.Keys.Select(i => $"{LabelFor(i)} = {GetCoefficientExpressionString(i)}"));
            lines.Add("for any {" + String.Join(", ", _freeCoefficientIndices.Select(LabelFor)) + "}");

            return lines;
        }
    }

    public override String ToString(OutputFormat format)
    {
        if (format != OutputFormat.VectorsNotation) return base.ToString(format);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (_dependentCoefficientExpressions == null || _freeCoefficientIndices == null) return "<FAIL>";

        return DegreesOfFreedom
             + ":{"
             + String.Join(", "
                         , Enumerable.Range(0, Equation.SubstancesCount)
                                     .Select(i => _freeCoefficientIndices.Contains(i) ? LabelFor(i) : GetCoefficientExpressionString(i)))
             + '}';
    }

    public String EquationWithPlaceholders() =>
        Utils.AssembleEquationString(Enumerable.Range(0, SubstancesCount).Select(LabelFor).ToArray()
                                   , static _ => true
                                   , static value => value + Settings.Default.MULTIPLICATION_SYMBOL
                                   , GetSubstance
                                   , static _ => true
                                   , true);

    public String LabelFor(Int32 i) => SubstancesCount > Settings.Default.LETTER_LABEL_THRESHOLD ? Utils.GenericLabel(i) : Utils.LetterLabel(i);

    // TODO: this has a lot of common logic with EquationWithIntegerCoefficients that calls AssembleEquationString<T>, but the output is much better in context. Try to generalize?
    // "a = c/2" vs "2a = c" thing
    public String? GetCoefficientExpressionString(Int32 index)
    {
        if (_dependentCoefficientExpressions == null) throw new InvalidOperationException();

        if (!_dependentCoefficientExpressions.ContainsKey(index)) return null;

        var expression = _dependentCoefficientExpressions[index];

        var numeratorParts = Enumerable.Range(index + 1, expression.Length - (index + 1))
                                       .Where(i => expression[i] != 0)
                                       .Select(i =>
                                               {
                                                   var coefficient = expression[i] + Settings.Default.MULTIPLICATION_SYMBOL;
                                                   if (expression[i] == 1) coefficient = String.Empty;
                                                   if (expression[i] == -1) coefficient = "-";
                                                   return $"{coefficient}{LabelFor(i)}";
                                               })
                                       .ToList();

        var result = String.Join(" + ", numeratorParts).Replace("+ -", "- ");

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
        if (parameters.Length != _freeCoefficientIndices!.Count) throw new ArgumentOutOfRangeException(nameof(parameters), "Array size mismatch");

        var result = new BigInteger[SubstancesCount];

        for (var i = 0; i < _freeCoefficientIndices.Count; i++)
        {
            result[_freeCoefficientIndices[i]] = parameters[i];
        }

        foreach (var kvp in _dependentCoefficientExpressions!)
        {
            var calculated = _freeCoefficientIndices.Aggregate(BigInteger.Zero, (sum, i) => sum + result[i] * kvp.Value[i]);
            BalancerException.ThrowIf(calculated % kvp.Value[kvp.Key] != 0, "Non-integer coefficient, try other SLE params");
            calculated /= kvp.Value[kvp.Key];

            result[kvp.Key] = -calculated;
        }

        return (result, EquationWithIntegerCoefficients(result));
    }

    protected override void BalanceImplementation()
    {
        var reducedMatrix = RationalMatrix.CreateInstance(Equation.CompositionMatrix.Reduce(), static r => r);
        BalancerException.ThrowIf(reducedMatrix.IsIdentityMatrix, "SLE is unsolvable");


        Details.AddRange(reducedMatrix.PrettyPrint("Reduced matrix"));

        _dependentCoefficientExpressions = Enumerable.Range(0, reducedMatrix.RowCount)
                                                     .Select(r => Utils.ScaleRationals(reducedMatrix.GetRow(r)).Select(static i => -i).ToArray())
                                                     .ToDictionary(static row => Array.FindIndex(row, static i => i != 0), static row => row);

        _freeCoefficientIndices = Enumerable.Range(0, reducedMatrix.ColumnCount)
                                            .Where(c => !_dependentCoefficientExpressions.ContainsKey(c) && !reducedMatrix.IsColumnAllZeroes(c))
                                            .ToList();
    }
}
