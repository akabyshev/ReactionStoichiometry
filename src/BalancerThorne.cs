﻿namespace ReactionStoichiometry;

using System.Numerics;
using MathNet.Numerics.LinearAlgebra;

internal sealed class BalancerThorne : Balancer
{
    private List<BigInteger[]>? _independentEquations;

    public BalancerThorne(String equation) : base(equation)
    {
    }

    protected override IEnumerable<String> Outcome
    {
        get
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (_independentEquations == null) return new[] { Program.FAILED_BALANCING_OUTCOME };

            return _independentEquations.Select(EquationWithIntegerCoefficients);
        }
    }

    protected override void BalanceImplementation()
    {
        var augmentedMatrix = GetAugmentedMatrix();
        if (!Utils.IsNonZeroDouble(augmentedMatrix.Determinant())) throw new BalancerException("Augmented matrix can't be inverted");
        var inverse = augmentedMatrix.Inverse();
        Details.AddRange(Utils.PrettyPrintMatrix("Inverse of the augmented matrix", inverse.ToArray()));


        _independentEquations = Enumerable.Range(inverse.ColumnCount - M.Nullity(), M.Nullity())
                                          .Select(c => Utils.ScaleDoubles(inverse.Column(c).ToArray()))
                                          .ToList();
    }

    private Matrix<Double> GetAugmentedMatrix()
    {
        var reduced = M.RowCount == M.ColumnCount ? SpecialMatrixReducedDouble.CreateInstance(M).ToMatrix() : M.Clone();

        if (reduced.RowCount == reduced.ColumnCount) throw new BalancerException("Matrix in RREF is still square");

        var nullity = reduced.Nullity();
        var submatrixLeftZeroes = Matrix<Double>.Build.Dense(nullity, reduced.ColumnCount - nullity);
        var submatrixRightIdentity = Matrix<Double>.Build.DenseIdentity(nullity);
        var result = reduced.Stack(submatrixLeftZeroes.Append(submatrixRightIdentity));

        result.CoerceZero(Program.GOOD_ENOUGH_DOUBLE_ZERO);

        return result;
    }
}