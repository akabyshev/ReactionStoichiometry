using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rationals;

namespace ReactionStoichiometry
{
    internal sealed class RationalArrayJsonConverter : JsonConverter<Rational[,]>
    {
        public override void WriteJson(JsonWriter writer, Rational[,]? data, JsonSerializer serializer)
        {
            ArgumentNullException.ThrowIfNull(data);

            var jArray = new JArray(Enumerable.Range(start: 0, data.RowCount())
                                              .Select(selector: r => new JArray(Enumerable.Range(start: 0, data.ColumnCount())
                                                                                          .Select(
                                                                                              selector:
                                                                                              c => $"{data[r, c].Numerator}/{data[r, c].Denominator}"))));

            jArray.WriteTo(writer);
        }

        public override Rational[,] ReadJson(JsonReader reader
                                           , Type objectType
                                           , Rational[,]? existingValue
                                           , Boolean hasExistingValue
                                           , JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
