using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Infrastructure.Json.Core;
using ImagineCommunications.GamePlan.Infrastructure.Json.Core.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace xggameplan.utils.seeddata.Infrastructure
{
    public class JsonDocumentImporter<T> : JsonDomainModelContextImporter<string, List<T>>
        where T : class
    {
        public JsonDocumentImporter(IDomainModelContext domainModelDbContext)
            : base(domainModelDbContext)
        {
        }

        protected override JsonReader CreateJsonReader(string source) => new JsonTextReader(new StringReader(source));

        protected override JsonSerializerSettings CreateSerializerSettings() =>
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                ContractResolver = new PrivateSetterContractResolver(),
                Converters = new List<JsonConverter> { new NodaTimeDurationTicksJsonConverter() }
            };

        protected override IEnumerable<object> ProcessDeserializedData(JsonSerializer serializer, List<T> data)
        {
            return data;
        }
    }

    internal class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (!property.Writable)
            {
                property.Writable = (member as PropertyInfo)?.SetMethod != null;
            }

            return property;
        }
    }
}
