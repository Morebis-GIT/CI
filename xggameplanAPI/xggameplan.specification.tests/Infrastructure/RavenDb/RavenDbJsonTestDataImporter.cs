using System.Collections.Generic;
using BoDi;
using xggameplan.specification.tests.Infrastructure.Json;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
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
            base.StoreDeserializedData(data);
            _scenarioDbContext.WaitForIndexesToBeFresh();
        }
    }
}
