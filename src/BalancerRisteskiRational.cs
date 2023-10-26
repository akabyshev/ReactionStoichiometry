using MathNet.Numerics.LinearAlgebra;
using Rationals;

namespace ReactionStoichiometry;

internal class BalancerRisteskiRational : AbstractBalancerRisteski<Rational>
{
    public BalancerRisteskiRational(string equation) : base(equation)
    {
    }

    protected override long[] ScaleToIntegers(Rational[] v)
    {
        return Helpers.ScaleRationals(v);
    }

    protected override ReducedMatrixOfRational GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
        return ReducedMatrixOfRational.CreateInstance(augmentedMatrix);
    }

    protected override string PrettyPrinter(Rational value)
    {
        return Helpers.PrettyPrintRational(value);
    }
}