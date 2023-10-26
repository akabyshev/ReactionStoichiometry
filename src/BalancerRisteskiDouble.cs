using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class BalancerRisteskiDouble : AbstractBalancerRisteski<double>
{
    public BalancerRisteskiDouble(string equation) : base(equation)
    {
    }

    protected override long[] ScaleToIntegers(double[] v)
    {
        return Helpers.ScaleDoubles(v);
    }

    protected override DoubleMatrixInRREF GetReducedAugmentedMatrix()
    {
        var augmented_matrix = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
        return new DoubleMatrixInRREF(augmented_matrix);
    }

    protected override string PrettyPrinter(double value)
    {
        return Helpers.PrettyPrintDouble(value);
    }
}