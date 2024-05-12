using System.Numerics;

namespace ReactionStoichiometry.Tests
{
    public sealed class ColumnsBasedSolutionTests
    {
        [Fact]
        public void TrueNegative_Simple()
        {
            const String eqUnsolvable = "FeS2+HNO3=Fe2(SO4)3+NO+H2SO4";
            var solution = new ChemicalReactionEquation(eqUnsolvable).ColumnsBasedSolution;
            Assert.False(solution.Success);
            Assert.Equal(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.Simple));
            Assert.Contains(GlobalConstants.FAILURE_MARK, solution.ToString(OutputFormat.DetailedMultiline));
            Assert.Null(solution.IndependentSetsOfCoefficients);
            Assert.Null(solution.CombinationSample.recipe);
            Assert.Null(solution.CombinationSample.coefficients);
        }

        [Fact]
        public void TruePositive_Simple()
        {
            const String eq = "H2 + O2 = H2O";
            var equation = new ChemicalReactionEquation(eq);
            Assert.True(equation.ColumnsBasedSolution.Success);
            Assert.Equal(expected: "x01·H2 + x02·O2 + x03·H2O = 0 with coefficients {-2, -1, 2}", equation.ColumnsBasedSolution.ToString(OutputFormat.Simple));
        }

        [Fact]
        public void ValidateSolution_Batch()
        {
            using StreamReader reader = new(path: @".\70_from_the_book.txt");

            while (reader.ReadLine() is { } line)
            {
                if (line.StartsWith(value: '#'))
                {
                    continue;
                }

                var equation = new ChemicalReactionEquation(line);
                Assert.True(equation.ColumnsBasedSolution.Success);
                Assert.NotNull(equation.ColumnsBasedSolution.IndependentSetsOfCoefficients);

                foreach (var coefficients in equation.ColumnsBasedSolution.IndependentSetsOfCoefficients)
                {
                    Assert.True(equation.Validate(coefficients));
                }
            }
        }

        [Fact]
        public void CombinationsOfTwo()
        {
            var solution = new ChemicalReactionEquation(equationString: "C6H5C2H5 + O2 = C6H5OH + CO2 + H2O").ColumnsBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Equal(expected: 2, solution.IndependentSetsOfCoefficients.Count);
            Assert.Equal(new BigInteger[] { 6, -7, -10, 12, 0 }, solution.IndependentSetsOfCoefficients[index: 0]);
            Assert.Equal(new BigInteger[] { -6, -7, 8, 0, 6 }, solution.IndependentSetsOfCoefficients[index: 1]);

            // all-zero
            Assert.Equal(new BigInteger[] { 0, 0, 0, 0, 0 }, solution.CombineIndependents(0, 0));

            // C6H5C2H5 is -6 and +6        others reduced /2
            Assert.Equal(new BigInteger[] { 0, -7, -1, 6, 3 }, solution.CombineIndependents(1, 1));

            // C6H5OH is -10*4 and +8*5     others reduced /3 :
            Assert.Equal(new BigInteger[] { -2, -21, 0, 16, 10 }, solution.CombineIndependents(4, 5));

            // non-zero                     non-reducible
            Assert.Equal(new BigInteger[] { -6, -35, 4, 24, 18 }, solution.CombineIndependents(4, 6));
        }

        [Fact]
        public void CombinationsOfOne()
        {
            var solution = new ChemicalReactionEquation(equationString: "K4Fe(CN)6+H2SO4+H2O=K2SO4+FeSO4+(NH4)2SO4+CO").ColumnsBasedSolution;
            Assert.True(solution.Success);
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Single(solution.IndependentSetsOfCoefficients);
            Assert.Null(solution.CombinationSample.recipe);
            Assert.Null(solution.CombinationSample.coefficients);
        }

        [Fact]
        public void FindCombination_Simple()
        {
            var solution = new ChemicalReactionEquation(equationString: "CO+CO2+H2=CH4+H2O").ColumnsBasedSolution;
            Assert.NotNull(solution.IndependentSetsOfCoefficients);
            Assert.Equal(new BigInteger[] { -2, 1, -2, 1, 0 }, solution.IndependentSetsOfCoefficients[index: 0]);
            Assert.Equal(new BigInteger[] { 1, -1, -1, 0, 1 }, solution.IndependentSetsOfCoefficients[index: 1]);
            Assert.Equal(new[] { 0, 0 }, solution.FindCombination(0, 0, 0, 0, 0));
            Assert.Equal(new[] { 1, 0 }, solution.FindCombination(-2, 1, -2, 1, 0));
            Assert.Equal(new[] { 0, 1 }, solution.FindCombination(1, -1, -1, 0, 1));
            Assert.Equal(new[] { 1, 3 }, solution.FindCombination(1, -2, -5, 1, 3));
            Assert.Equal(new[] { 7, 13 }, solution.FindCombination(-1, -6, -27, 7, 13));
            Assert.Equal(new[] { 13, 7 }, solution.FindCombination(-19, 6, -33, 13, 7));
            Assert.Null(solution.FindCombination(-1, -1, -1, -1, 1377));
        }

        [Fact]
        public void FindCombination_Complex()
        {
            const String veryLong =
                "CaBeAsSAtCsF13 + (Ru(C10H8N2)3)Cl2(H2O)6 + W2Cl8(NSeInCl3)2 + Ca(GaH2S4)2 + (NH4)2MoO4 + K4Fe(CN)6 + Na2Cr2O7 + MgS2O3 + LaTlS3 + Na3PO4 + Ag2PbO2 + SnSO4 + HoHS4 + CeCl3 + ZrO2 + Cu2O + Al2O3 + Bi2O3 + SiO2 + Au2O + TeO3 + CdO + Hg2S = (NH3)3((PO)4(MoO3)12) + LaHgTlZrS6 + In3CdCeCl12 + AgRuAuTe8 + C4H3AuNa2OS7 + KAu(CN)2 + MgFe2(SO4)4 + Sn3(AsO4)3BiAt3 + CuCsCl3 + GaHoH2S4 + N2SiSe6 + CaAl0.97F5 + PbCrO4 + H2CO3 + BeSiO3 + HClO + W2O";
            var equation = new ChemicalReactionEquation(veryLong);
            var cbs = equation.ColumnsBasedSolution;
            Assert.NotNull(cbs.IndependentSetsOfCoefficients);
            var fromRbs = new BigInteger[]
                          {
                              -13464177000
                            , -3711371760
                            , -4709969640
                            , -21542683200
                            , 833061840
                            , -13545165930
                            , -927842940
                            , -6772582965
                            , -72858187812
                            , 277687280
                            , -1855685880
                            , -13464177000
                            , -43085366400
                            , -3139979760
                            , -72858187812
                            , -6732088500
                            , -16978327197
                            , -2244029500
                            , -15034166880
                            , -29201673750
                            , -29690974080
                            , -3139979760
                            , -36429093906
                            , -69421820
                            , 72858187812
                            , 3139979760
                            , 3711371760
                            , 511312020
                            , 54180663720
                            , 6772582965
                            , 4488059000
                            , 13464177000
                            , 43085366400
                            , 1569989880
                            , 35006860200
                            , 1855685880
                            , 82205572860
                            , 13464177000
                            , 4709969640
                            , 4709969640
                          };
            Assert.True(equation.Validate(fromRbs));
            Assert.Equal(new[] { 3, 40 }, cbs.FindCombination(fromRbs));
            var fromAuthor = new BigInteger[]
                             {
                                 -7731000
                               , -1502160
                               , -9273600
                               , -12369600
                               , -1560720
                               , -12054510
                               , -375540
                               , -6027255
                               , -37709196
                               , -520240
                               , -751080
                               , -7731000
                               , -24739200
                               , -6182400
                               , -37709196
                               , -3865500
                               , -9748791
                               , -1288500
                               , -10822200
                               , -25438050
                               , -12017280
                               , -6182400
                               , -18854598
                               , 130060
                               , 37709196
                               , 6182400
                               , 1502160
                               , 1155900
                               , 48218040
                               , 6027255
                               , 2577000
                               , 7731000
                               , 24739200
                               , 3091200
                               , 20100600
                               , 751080
                               , 16332180
                               , 7731000
                               , 54000120
                               , 9273600
                             };
            Assert.True(equation.Validate(fromAuthor));
            Assert.Null(cbs.FindCombination(fromAuthor)); // todo: how did he get that valid-but-invalid solution?
        }
    }
}
