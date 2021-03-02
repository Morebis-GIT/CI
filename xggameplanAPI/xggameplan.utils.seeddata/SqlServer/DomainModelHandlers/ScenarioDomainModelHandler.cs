using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios.Scenario;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ScenarioDomainModelHandler : IDomainModelHandler<Scenario>
    {
        private readonly IScenarioRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public ScenarioDomainModelHandler(
            IScenarioRepository repository,
            ISqlServerTenantDbContext dbContext
        )
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public Scenario Add(Scenario model)
        {
            _repository.Add(model);

            return model;
        }

        public void AddRange(params Scenario[] models) => _repository.Add(models);

        public int Count() => _dbContext
            .Query<ScenarioEntity>()
            .Count();

        public void DeleteAll() => _dbContext
            .Truncate<ScenarioEntity>();

        public IEnumerable<Scenario> GetAll() => _repository.GetAll();
    }
}
