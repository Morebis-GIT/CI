using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BoDi;
using ImagineCommunications.GamePlan.Infrastructure.Json.Core;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Json
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
            var deserializedData = data.SelectMany(x =>
             {
                 return DeserializeModels(serializer, _objectContainer.Resolve<IImportedModel>(x.Key), x.Value);
             });

            return deserializedData;
        }

        protected virtual object[] DeserializeModels(JsonSerializer serializer, IImportedModel importedModel, params JObject[] objects)
        {
            return objects.Select(o => o.ToObject(importedModel.Type, serializer)).ToArray();
        }

        public IEnumerable<TEntity> GetDataFromFile<TEntity>(string fileName, string key) where TEntity : class
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(assembly.GetFullManifestResourceName(fileName.EndsWith(".json") ? fileName : fileName + ".json")))
            {
                using (var jsonReader = CreateJsonReader(stream))
                {
                    var serializer = CreateJsonSerializer();
                    var deserializedData = serializer.Deserialize<Dictionary<string, JObject[]>>(jsonReader);
                    if (deserializedData != null)
                    {
                        var objects = deserializedData[key];
                        if (objects == null)
                        {
                            throw new InvalidDataException($"The key: {key} not found in file: {fileName}");
                        }
                        var result = objects.Select(o => o.ToObject(typeof(TEntity), serializer) as TEntity);
                        return result;
                    }

                    return null;
                }
            }
        }

        public void ImportDataFromTable(string key, Table table)
        {
            var importedType = _objectContainer.Resolve<IImportedModel>(key);
            var listToImport = importedType.CreateSet(table);
            StoreDeserializedData(listToImport);
        }
    }
}
