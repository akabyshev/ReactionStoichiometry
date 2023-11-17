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
            var balancer = new BalancerGeneralized(eq);
            Assert.Equal(GlobalConstants.FAILURE_MARK, balancer.ToString(OutputFormat.Simple));
            Assert.NotEqual(GlobalConstants.FAILURE_MARK, balancer.ToString(OutputFormat.Json));

            Assert.True(balancer.Balance());
            Assert.NotEqual(GlobalConstants.FAILURE_MARK, balancer.ToString(OutputFormat.Simple));
            Assert.Null(Record.Exception(testCode: () => _ = balancer.ToString(OutputFormat.Json)));
        }

        [Fact]
        public void JsonDeserializationExceptions_Simple()
        {
            Assert.Throws<JsonReaderException>(testCode: () => { _ = JsonConvert.DeserializeObject<Balancer>(value: "some random string"); });

            const String randomJsonString = "{\"Name\":\"John\",\"Age\":30}";
            Assert.Throws<JsonSerializationException>(testCode: () => { _ = JsonConvert.DeserializeObject<Balancer>(randomJsonString); });

            const String eq = "H2 + O2 = H2O";
            var balancer = new BalancerGeneralized(eq);
            var converter = new RationalArrayJsonConverter();
            Assert.Throws<NotImplementedException>(testCode: () =>
                                                             {
                                                                 _ = JsonConvert.DeserializeObject<Rational[,]>(
                                                                     JsonConvert.SerializeObject(balancer.Equation.CCM, converter)
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
