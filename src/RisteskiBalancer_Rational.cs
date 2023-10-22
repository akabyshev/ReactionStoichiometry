using MathNet.Numerics.LinearAlgebra;
using Rationals;

namespace ReactionStoichiometry;

internal class RisteskiBalancer_Rational : RisteskiBalancer<Rational>
{
    public RisteskiBalancer_Rational(string equation) : base(equation)
    {
    }

    protected override long[] VectorScaler(Rational[] v)
    {
        return Helpers.ScaleRationals(v);
    }

    protected override RationalMatrixInRREF GetReducedAugmentedMatrix()
    {
        var AM = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
        return new RationalMatrixInRREF(AM);
    }

    protected override string PrettyPrinter(Rational value)
    {
        return Helpers.PrettyPrintRational(value);
    }
}