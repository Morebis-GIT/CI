using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ScenarioCampaignFailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioCampaignFailure;
using xggameplan.specification.tests.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class ScenarioCampaignFailureDomainModelHandler : SimpleDomainModelMappingHandler<ScenarioCampaignFailureEntity, ScenarioCampaignFailure>
    {
        private readonly ITenantDbContext _dbContext;

        public ScenarioCampaignFailureDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override ScenarioCampaignFailure Add(ScenarioCampaignFailure model)
        {
            var entity = Mapper.Map<ScenarioCampaignFailureEntity>(model);
            _ = _dbContext.Add(entity);
            return model;
        }

        public override void AddRange(params ScenarioCampaignFailure[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public override IEnumerable<ScenarioCampaignFailure> GetAll()
        {
            var entities = _dbContext.Query<ScenarioCampaignFailureEntity>().ToList();
            return Mapper.Map<List<ScenarioCampaignFailure>>(entities);
        }
    }
}
