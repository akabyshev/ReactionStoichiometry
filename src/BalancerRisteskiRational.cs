using MathNet.Numerics.LinearAlgebra;
using Rationals;

namespace ReactionStoichiometry;

internal class BalancerRisteskiRational : AbstractBalancerRisteski<Rational>
{
    public BalancerRisteskiRational(string equation) : base(equation)
    {
    }

    protected override long[] ScaleVectorToIntegers(Rational[] v)
    {
        return Helpers.ScaleRationals(v);
    }

    protected override RationalMatrixInRREF GetReducedAugmentedMatrix()
    {
        var augmented_matrix = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
        return new RationalMatrixInRREF(augmented_matrix);
    }

    protected override string PrettyPrinter(Rational value)
    {
        return Helpers.PrettyPrintRational(value);
    }
}