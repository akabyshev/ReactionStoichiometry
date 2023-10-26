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
        return Utils.ScaleRationals(v);
    }

    protected override ReducedMatrixOfRational GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = M.Append(Matrix<double>.Build.Dense(M.RowCount, 1));
        return ReducedMatrixOfRational.CreateInstance(augmentedMatrix);
    }

    protected override string PrettyPrinter(Rational value)
    {
        return Utils.PrettyPrintRational(value);
    }
}