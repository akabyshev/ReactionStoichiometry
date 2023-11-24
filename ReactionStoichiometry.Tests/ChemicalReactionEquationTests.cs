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
        public void SubstancesMustBeUnique_Simple()
        {
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "H+H=H2")));
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "Ag + Au = Au")));
            Assert.Null(Record.Exception(testCode: () => _ = new ChemicalReactionEquation(equationString: "Na + Cl2 = NaCl + NaCl")));
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
