using System;
using Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Infrastructure.Json.Core.Converters
{
    /// <summary>Converts DateTime to only return a Date value</summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class OnlyDateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType);
        }

        /// <summary>  Throws <see cref="NotImplementedException"/> as <see cref="OnlyDateConverter"/> has no input needed. Strictly output only.</summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from</param>
        /// <param name="objectType">Type of the object</param>
        /// <param name="existingValue">The existing value of object being read</param>
        /// <param name="serializer">The calling serializer</param>
        /// <returns>The object value</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = (DateTime)value;
            writer.WriteValue(date.Date);
        }
    }
}
