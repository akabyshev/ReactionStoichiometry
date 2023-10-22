using MathNet.Numerics.LinearAlgebra;

namespace ReactionStoichiometry;

internal class RisteskiBalancer_Double : RisteskiBalancer<double>
{
    public RisteskiBalancer_Double(string equation) : base(equation)
    {
    }

    protected override long[] VectorScaler(double[] v)
    {
        return Helpers.ScaleDoubles(v);
    }

    protected override DoubleMatrixInRREF GetReducedAugmentedMatrix()
    {
        var AM = matrix.Append(Matrix<double>.Build.Dense(matrix.RowCount, 1));
        return new DoubleMatrixInRREF(AM);
    }

    protected override string PrettyPrinter(double value)
    {
        return Helpers.PrettyPrintDouble(value);
    }
}