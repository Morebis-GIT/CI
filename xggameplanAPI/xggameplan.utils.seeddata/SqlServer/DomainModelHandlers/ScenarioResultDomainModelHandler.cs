using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ScenarioResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioResult;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ScenarioResultDomainModelHandler : IDomainModelHandler<ScenarioResult>
    {
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly ISqlServerDbContext _dbContext;

        public ScenarioResultDomainModelHandler(IScenarioResultRepository scenarioResultRepository, ISqlServerDbContext dbContext)
        {
            _scenarioResultRepository = scenarioResultRepository ??
                                       throw new ArgumentNullException(nameof(scenarioResultRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ScenarioResult Add(ScenarioResult model)
        {
            _scenarioResultRepository.Add(model);
            return model;
        }

        public void AddRange(params ScenarioResult[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<ScenarioResultEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<ScenarioResultEntity>();

        public IEnumerable<ScenarioResult> GetAll() => _scenarioResultRepository.GetAll();
    }
}
