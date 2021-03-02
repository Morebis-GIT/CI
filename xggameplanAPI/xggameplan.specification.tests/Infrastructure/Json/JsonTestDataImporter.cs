using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BoDi;
using ImagineCommunications.GamePlan.Infrastructure.Json.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.Json
{
    public class JsonTestDataImporter : JsonDomainModelContextImporter<Stream, Dictionary<string, JObject[]>>, ITestDataImporter
    {
        private readonly IObjectContainer _objectContainer;

        protected JsonTestDataImporter(IScenarioDbContext scenarioDbContext, IObjectContainer objectContainer) : base(scenarioDbContext)
        {
            _objectContainer = objectContainer;
        }

        protected override JsonReader CreateJsonReader(Stream source)
        {
            return new JsonTextReader(new StreamReader(source, Encoding.UTF8));
        }

        protected override IEnumerable<object> ProcessDeserializedData(JsonSerializer serializer, Dictionary<string, JObject[]> data)
        {
            return data.SelectMany(x =>
            {
                return DeserializeModels(serializer, _objectContainer.Resolve<IImportedModel>(x.Key), x.Value);
            });
        }

        protected virtual object[] DeserializeModels(JsonSerializer serializer, IImportedModel importedModel, params JObject[] objects)
        {
            return objects.Select(o => o.ToObject(importedModel.Type, serializer)).ToArray();
        }
    }
}
