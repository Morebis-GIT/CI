using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using xggameplan.specification.tests.Interfaces;
using ScenarioCampaignResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignResult;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class ScenarioCampaignResultDomainModelHandler : SimpleDomainModelMappingHandler<ScenarioCampaignResultEntity, ScenarioCampaignResult>
    {
        private readonly ITenantDbContext _dbContext;

        public ScenarioCampaignResultDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override ScenarioCampaignResult Add(ScenarioCampaignResult model)
        {
            _dbContext.AddRange(Mapper.Map<List<ScenarioCampaignResultEntity>>(model));

            return model;
        }

        public override void AddRange(params ScenarioCampaignResult[] models)
        {
            var entities = models.SelectMany(x => Mapper.Map<List<ScenarioCampaignResultEntity>>(x)).ToArray();

            _dbContext.AddRange(entities);
        }

        public override IEnumerable<ScenarioCampaignResult> GetAll() => _dbContext
            .Query<ScenarioCampaignResultEntity>()
            .GroupBy(e => e.ScenarioId)
            .Select(e => new ScenarioCampaignResult
            {
                Id = e.Key,
                Items = Mapper.Map<List<ScenarioCampaignResultItem>>(e.ToList())
            })
            .ToArray();
    }
}
