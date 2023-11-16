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
            Assert.Equal(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.Simple));
            Assert.NotEqual(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.Json));

            Assert.True(balancer.Balance());
            Assert.NotEqual(GlobalConstants.FAILURE_MARK, balancer.ToString(Balancer.OutputFormat.Simple));
            Assert.Null(Record.Exception(testCode: () => _ = balancer.ToString(Balancer.OutputFormat.Json)));
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
            const String eq = "H2 + O2 = H2O";
            const String expectedSerialization = "[[\"2\",\"0\",\"2\"],[\"0\",\"2\",\"1\"]]";
            var ccm = new BalancerGeneralized(eq).Equation.CCM;
            var json = JsonConvert.SerializeObject(ccm, new RationalArrayJsonConverter());
            Assert.Equal(expectedSerialization, json);
        }
    }
}
