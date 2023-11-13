using Rationals;
// ReSharper disable InconsistentNaming
// ReSharper disable ArgumentsStyleLiteral

namespace ReactionStoichiometryTests
{
    public sealed class MathTests
    {
        // Use https://matrixcalc.org or https://www.desmos.com/matrix for Inverse and Determinant
        // Use https://www.wolframalpha.com/input/?i=matrix+rank+calculator for Rank
        private readonly Rational[,] _knownMatrix = { { 0, 2, 3, 4 }, { 5, 60, 7, 8 }, { 9, 10, 11, 12 } };

        private readonly Rational[,] _knownMatrixRREF =
        {
            { 1, 0, 0, new Rational(-216, 727) }, { 0, 1, 0, new Rational(2, 727) }, { 0, 0, 1, new Rational(968, 727) }
        };

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
            var calculatedRREF = (Rational[,]) _knownMatrix.Clone();
            calculatedRREF.TurnIntoRREF(); // todo: encapsulate to one-liner
            RationalMatrixOperations.TrimAndGetCanonicalForms(ref calculatedRREF);

            Assert.Equal(_knownMatrixRREF, calculatedRREF);
            var calculatedRank = calculatedRREF.RowCount();
            Assert.Equal(_knownMatrixRank, calculatedRank);
        }
    }
}
