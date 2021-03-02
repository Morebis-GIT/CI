using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Extensions;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Steps.DataStorage
{
    [Binding]
    public class RepositoryDefinitionSteps
    {
        private const string PredefinedDataKey = "predefinedData";

        private readonly ScenarioContext _scenarioContext;
        private readonly ITestDataImporter _testDataImporter;

        public RepositoryDefinitionSteps(ScenarioContext scenarioContext, ITestDataImporter testDataImporter)
        {
            _scenarioContext = scenarioContext;
            _testDataImporter = testDataImporter;
        }

        private string ToCacheKey(string value) =>
            value + _scenarioContext.ScenarioInfo.Title.GetHashCode();

        private T CacheRead<T>(string key) =>
            _scenarioContext.TryGetValue(ToCacheKey(key), out T item)
                ? item
                : throw new InvalidOperationException($"Could not find the key {key} in the scenario cache");

        private void CacheWrite<T>(string key, T item) =>
            _scenarioContext.Set(item, ToCacheKey(key));

        [Given(@"predefined (.*) data")]
        public void GivenPredefinedData(string name)
        {
            if (!_scenarioContext.TryGetValue<ICollection<string>>(ToCacheKey(PredefinedDataKey), out var dataList))
            {
                dataList = new List<string>();
                CacheWrite(PredefinedDataKey, dataList);
            }

            dataList.Add(name);
        }

        [Given(@"predefined (.*) data imported")]
        public void GivenPredefinedDataImported(string fileName)
        {
            var name = CacheRead<ICollection<string>>(PredefinedDataKey)?.FirstOrDefault(x => x == fileName);

            if (name is null)
            {
                throw new ArgumentException($"'{fileName}' file doesn't exist.", nameof(fileName));
            }

            _testDataImporter.FromResourceScript(Path.HasExtension(name)
                ? name
                : Path.ChangeExtension(name, ".json"));
        }

        [Given(@"predefined data imported")]
        public void GivenPredefinedDataImported()
        {
            var dataList = CacheRead<ICollection<string>>(PredefinedDataKey);

            foreach (var name in dataList)
            {
                _testDataImporter.FromResourceScript(Path.HasExtension(name)
                    ? name
                    : Path.ChangeExtension(name, ".json"));
            }
        }

        [Given(@"there is a (.*) repository")]
        public void GivenRepository(string repositoryName)
        {
            var adapter = _scenarioContext.ScenarioContainer.Resolve<IRepositoryAdapter>(repositoryName);
            _scenarioContext.ScenarioContainer.RegisterInstanceAs(adapter);
        }

        [Given(@"(\d+) documents created")]
        public void GivenRepositoryWithDocuments(int documentCount)
        {
            Assert.GreaterOrEqual(documentCount, 1);
            var adapter = _scenarioContext.ScenarioContainer.Resolve<IRepositoryAdapter>();
            _ = adapter.ExecuteGivenAutoAdd(documentCount).ThrowExceptionIfExists();
        }

        [Given(@"the following documents created:")]
        public void GivenTheFollowingDocumentsAdded(Table entities)
        {
            var adapter = _scenarioContext.ScenarioContainer.Resolve<IRepositoryAdapter>();
            _ = adapter.ExecuteGivenAddRange(entities).ThrowExceptionIfExists();
        }
    }
}
