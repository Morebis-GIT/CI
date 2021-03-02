using System.Collections;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace ImagineCommunications.GamePlan.Infrastructure.Json.Core
{
    public abstract class JsonImporter<TSource, TDeserializedType> : IImporter<TSource>
        where TDeserializedType : IEnumerable
    {
        protected JsonImporter()
        {
        }

        protected abstract JsonReader CreateJsonReader(TSource source);

        protected abstract IEnumerable<object> ProcessDeserializedData(JsonSerializer serializer, TDeserializedType data);

        protected abstract void StoreDeserializedData(IEnumerable<object> data);

        protected virtual JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = new JsonSerializerSettings();
            _ = settings.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            return settings;
        }

        protected virtual JsonSerializer CreateJsonSerializer()
        {
            return JsonSerializer.CreateDefault(CreateSerializerSettings());
        }

        public void Import(TSource source)
        {
            using (var jsonReader = CreateJsonReader(source))
            {
                var serializer = CreateJsonSerializer();
                var deserializedData = serializer.Deserialize<TDeserializedType>(jsonReader);
                if (deserializedData != null)
                {
                    var data = ProcessDeserializedData(serializer, deserializedData);
                    if (data != null)
                    {
                        StoreDeserializedData(data);
                    }
                }
            }
        }
    }
}
