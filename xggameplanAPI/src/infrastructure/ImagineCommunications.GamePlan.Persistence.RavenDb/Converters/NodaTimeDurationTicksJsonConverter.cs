using System;
using NodaTime;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Linq;

namespace Raven.Client.NodaTime.JsonConverters
{
    public sealed class TicksWrapper
    {
        public TicksWrapper(long ticks)
        {
            Ticks = ticks;
        }

        [JsonProperty("ticks")]
        public long Ticks { get; }
    }

    public class NodaTimeDurationTicksJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Duration);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var handlingDuration = JObject.Load(reader);

            if (handlingDuration.ToString().Contains("ticks"))
            {
                var ticks = handlingDuration.First.ToObject<long>();

                return Duration.FromTicks(ticks);
            }

            if (handlingDuration.ToString().Contains("nanoOfDay"))
            {
                var nanoOfDay = handlingDuration.GetValue("nanoOfDay", StringComparison.InvariantCulture).ToObject<long>();

                return Duration.FromNanoseconds(nanoOfDay);
            }

            throw new JsonReaderException("JSON's text has invalid format.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var duration = (Duration)value;
            var ticksWrapper = new TicksWrapper(duration.BclCompatibleTicks);
            var wrapperObj = JToken.FromObject(ticksWrapper);

            wrapperObj.WriteTo(writer);
        }
    }
}
