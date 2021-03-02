using System.Collections.Generic;
using BoDi;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Json;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    public class RavenDbJsonTestDataImporter : JsonTestDataImporter
    {
        private readonly IScenarioDbContext _scenarioDbContext;

        public RavenDbJsonTestDataImporter(IScenarioDbContext scenarioDbContext, IObjectContainer objectContainer) : base(scenarioDbContext, objectContainer)
        {
            _scenarioDbContext = scenarioDbContext;
        }

        protected override void StoreDeserializedData(IEnumerable<object> data)
        {
            _scenarioDbContext.WaitForIndexesAfterSaveChanges();
            base.StoreDeserializedData(data);
            _scenarioDbContext.WaitForIndexesToBeFresh();
        }
    }
}
