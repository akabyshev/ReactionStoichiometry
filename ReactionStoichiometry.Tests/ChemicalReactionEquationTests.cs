using System.Numerics;

namespace ReactionStoichiometry.Tests
{
    public sealed class ChemicalReactionEquationTests
    {
        [Fact]
        public void Ctor_Simple()
        {
            Assert.Throws<AppSpecificException>(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2+O2=H2O:"));
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2 + O2 = H2O")));
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H2+O2=H2O")));
        }

        [Fact]
        public void LazyCalculations_Simple()
        {
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H+H=H2")));
            // this does not throw an exception because so Solutions are really calculated on that object

            Assert.Throws<IndexOutOfRangeException>(testCode: () => _ = new ChemicalReactionEquation(equationString: "H+H=H2").GeneralizedSolution);
            // here ctor throws an exception because GeneralizedSolution is actually created
        }

        [Fact]
        public void Validation_Simple()
        {
            var equation = new ChemicalReactionEquation(equationString: "H2+O2=H2O");
            Assert.True(equation.Validate(new BigInteger[] { -2, -1, 2 }));
            Assert.True(equation.Validate(new BigInteger[] { -4, -2, 4 }));
            Assert.False(equation.Validate(new BigInteger[] { -10, 7, -3 }));
            Assert.Throws<ArgumentException>(testCode: () => _ = equation.Validate(new BigInteger[] { -2, -1, 2, 2 }));
        }
    }
}
