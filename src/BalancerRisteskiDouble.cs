﻿namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;


internal sealed class BalancerRisteskiDouble : BalancerRisteski<Double>
{
    public BalancerRisteskiDouble(String equation) : base(equation)
    {
    }

    protected override BigInteger[] ScaleToIntegers(Double[] v) => Utils.ScaleDoubles(v);

    protected override SpecialMatrixReducedDouble GetReducedAugmentedMatrix()
    {
        var augmentedMatrix = M.Append(Matrix<Double>.Build.Dense(M.RowCount, 1));
        return SpecialMatrixReducedDouble.CreateInstance(augmentedMatrix);
    }

    protected override String PrettyPrinter(Double value) => Utils.PrettyPrintDouble(value);
}