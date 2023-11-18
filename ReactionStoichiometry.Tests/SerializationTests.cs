using Newtonsoft.Json;
using Rationals;

namespace ReactionStoichiometry.Tests
{
    public sealed class SerializationTests
    {
        [Fact]
        public void Json_Simple()
        {
            const String eq = "H2 + O2 = H2O";
            var equation = new ChemicalReactionEquation(eq);
            Assert.NotEqual(GlobalConstants.FAILURE_MARK, equation.GeneralizedSolution.ToString(OutputFormat.Simple));
            Assert.Null(Record.Exception(testCode: () => _ = equation.ToJson()));
        }

        [Fact]
        public void JsonDeserializationExceptions_Simple()
        {
            Assert.Throws<JsonReaderException>(testCode: () => { _ = JsonConvert.DeserializeObject<ChemicalReactionEquation>(value: "some random string"); });

            const String randomJsonString = "{\"Name\":\"John\",\"Age\":30}";
            Assert.Throws<NullReferenceException>(testCode: () => { _ = JsonConvert.DeserializeObject<ChemicalReactionEquation>(randomJsonString); });

            const String eq = "H2 + O2 = H2O";
            var equation = new ChemicalReactionEquation(eq);
            var converter = new RationalArrayJsonConverter();
            Assert.Throws<NotImplementedException>(testCode: () =>
                                                             {
                                                                 _ = JsonConvert.DeserializeObject<Rational[,]>(
                                                                     JsonConvert.SerializeObject(equation.CCM, converter)
                                                                   , converter);
                                                             });
        }

        [Fact]
        public void RationalMatrixJsonSerialization_Simple()
        {
            Rational[,] matrix = { { 2, 0, 2 }, { 0, 2, 1 } };
            const String expectedSerialization = "[[\"2\",\"0\",\"2\"],[\"0\",\"2\",\"1\"]]";
            var json = JsonConvert.SerializeObject(matrix, new RationalArrayJsonConverter());
            Assert.Equal(expectedSerialization, json);
        }
    }
}
