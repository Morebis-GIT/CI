using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using FailureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures.Failure;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class FailureDomainModelHandler : SimpleDomainModelMappingHandler<FailureEntity, Failures>
    {
        private readonly ITenantDbContext _dbContext;

        public FailureDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override Failures Add(Failures model)
        {
            _dbContext.AddRange(Mapper.Map<List<FailureEntity>>(model));

            return model;
        }

        public override void AddRange(params Failures[] models)
        {
            var entities = models.SelectMany(x => Mapper.Map<List<FailureEntity>>(x)).ToArray();

            _dbContext.AddRange(entities);
        }

        public override void DeleteAll() => _dbContext
            .Truncate<FailureEntity>();

        public override IEnumerable<Failures> GetAll() =>
            _dbContext
                .Query<FailureEntity>()
                .ProjectTo<Failures>(Mapper.ConfigurationProvider)
                .ToArray();
    }
}
