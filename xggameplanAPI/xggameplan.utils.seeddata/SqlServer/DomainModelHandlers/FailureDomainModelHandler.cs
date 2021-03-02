using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class FailureDomainModelHandler : IDomainModelHandler<Failures>
    {
        private readonly IFailuresRepository _repository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public FailureDomainModelHandler(
            IFailuresRepository repository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Failures Add(Failures model)
        {
            _repository.Add(model);

            return model;
        }

        public void AddRange(params Failures[] models)
        {
            foreach (var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext
            .Query<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures.Failure>()
            .Count();

        public void DeleteAll() => _dbContext
            .Truncate<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures.Failure>();

        public IEnumerable<Failures> GetAll() =>
            _dbContext
                .Query<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures.Failure>()
                .ProjectTo<Failures>(_mapper.ConfigurationProvider)
                .ToArray();
    }
}
