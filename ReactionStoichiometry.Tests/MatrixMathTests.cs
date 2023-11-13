using Rationals;
using System.Numerics;

// ReSharper disable InconsistentNaming
// ReSharper disable ArgumentsStyleLiteral

namespace ReactionStoichiometry.Tests
{
    public sealed class MatrixMathTests
    {
        // Use https://matrixcalc.org or https://www.desmos.com/matrix for Inverse and Determinant
        // Use https://www.wolframalpha.com/input/?i=matrix+rank+calculator for Rank
        private readonly Rational[,] _knownMatrix = { { 0, 2, 3, 4 }, { 5, 60, 7, 8 }, { 9, 10, 11, 12 } };

        private readonly Rational[,] _knownMatrixRREF = { { 1, 0, 0, new(-216, 727) }, { 0, 1, 0, new(2, 727) }, { 0, 0, 1, new(968, 727) } };

        private const Int32 _knownMatrixRank = 3;

        [Fact]
        public void InverseMatrix_Simple()
        {
            Rational[,] singularMatrix = { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } };
            Assert.Throws<AppSpecificException>(() => _ = RationalMatrixOperations.GetInverse(singularMatrix));

            Rational[,] nonSquareMatrix = { { 1, 2, 3, 4 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } };
            Assert.Throws<ArgumentException>(() => _ = RationalMatrixOperations.GetInverse(nonSquareMatrix));

            var augmentedMatrix = new Rational[4, 4];
            Array.Copy(_knownMatrix, augmentedMatrix, _knownMatrix.Length);
            augmentedMatrix[3, 0] = 13;
            augmentedMatrix[3, 1] = 14;
            augmentedMatrix[3, 2] = 15;
            augmentedMatrix[3, 3] = 16;

            var expectedInverse = new[,]
                                  {
                                      { -1, 0, 3, -2 }
                                    , { 0, new Rational(1, 54), new Rational(-1, 27), new Rational(1, 54) }
                                    , { 3, new Rational(-1, 27), new Rational(-349, 27), new Rational(242, 27) }
                                    , { -2, new Rational(1, 54), new Rational(1049, 108), new Rational(-727, 108) }
                                  };

            var calculatedInverse = RationalMatrixOperations.GetInverse(augmentedMatrix);
            Assert.Equal(expectedInverse, calculatedInverse);
        }

        [Fact]
        public void RREF_Simple()
        {
            var calculatedRREF = _knownMatrix.GetRREF();
            RationalMatrixOperations.TrimAndGetCanonicalForms(ref calculatedRREF);

            Assert.Equal(_knownMatrixRREF, calculatedRREF);
            var calculatedRank = calculatedRREF.RowCount();
            Assert.Equal(_knownMatrixRank, calculatedRank);
        }

        [Fact]
        public void TrimAndGetCanonicalForms_Simple()
        {
            Rational[,] matrix1 = { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            Assert.Throws<InvalidOperationException>(() => RationalMatrixOperations.TrimAndGetCanonicalForms(ref matrix1));

            Rational[,] matrix2 = { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
            Rational[,] matrix2_after = { { 0, 0, 0 }, { 0, 1, 0 } };

            Assert.Null(Record.Exception((Action)(() => RationalMatrixOperations.TrimAndGetCanonicalForms(ref matrix2))));
            Assert.Equal(matrix2_after, matrix2);

            Rational[,] matrix3 = { { 1, new(20, 10), new(300, 100) }, { new(4, 2), 0, new(64, 32) }, { 0, 0, 0 }, { 0, 0, 0 } };
            Rational[,] matrix3_after = { { 1, 2, 3 }, { 2, 0, 2 } };

            Assert.Null(Record.Exception((Action)(() => RationalMatrixOperations.TrimAndGetCanonicalForms(ref matrix3))));
            Assert.Equal(matrix3_after, matrix3);
        }

        [Fact]
        public void ScaleToIntegers_Simple()
        {
            Assert.Equal(0, RationalMatrixOperations.LeastCommonMultiple(0, 0));
            Assert.Equal(0, RationalMatrixOperations.LeastCommonMultiple(0, 60));
            Assert.Equal(0, RationalMatrixOperations.LeastCommonMultiple(60, 0));
            Assert.Equal(407, RationalMatrixOperations.LeastCommonMultiple(37, 11));
            Assert.Equal(240, RationalMatrixOperations.LeastCommonMultiple(60, 48));
            Assert.Equal(10, RationalMatrixOperations.LeastCommonMultiple(-5, 10));

            Assert.Equal(new BigInteger[] { 0, 0, 0 }, new Rational[] { 0, 0, 0 }.ScaleToIntegers());
            Assert.Equal(new BigInteger[] { 1, 2, 3 }, new Rational[] { 1, 2, 3 }.ScaleToIntegers());
            Assert.Equal(new BigInteger[] { 6, 3, 2 }, new Rational[] { 1, new(1, 2), new(1, 3) }.ScaleToIntegers());
            Assert.Equal(new BigInteger[] { 132, 11, 12, 0 }, new Rational[] { 1, new(2, 24), new(3, 33), 0 }.ScaleToIntegers());
        }

        [Fact]
        public void IsIdentityMatrix_Simple()
        {
            Assert.True(new Rational[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }.IsIdentityMatrix());
            Assert.False(new Rational[,] { { 2, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }.IsIdentityMatrix());
            Assert.False(new Rational[,] { { 1, 1, 0 }, { 0, 1, 0 }, { 0, 0, 1 } }.IsIdentityMatrix());
        }
    }
}
