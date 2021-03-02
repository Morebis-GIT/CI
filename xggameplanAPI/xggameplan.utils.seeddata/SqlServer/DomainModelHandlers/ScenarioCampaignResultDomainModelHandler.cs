using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ScenarioCampaignResultEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults.ScenarioCampaignResult;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ScenarioCampaignResultDomainModelHandler : IDomainModelHandler<ScenarioCampaignResult>
    {
        private readonly IScenarioCampaignResultRepository _repository;
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public ScenarioCampaignResultDomainModelHandler(
            IScenarioCampaignResultRepository repository,
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public ScenarioCampaignResult Add(ScenarioCampaignResult model)
        {
            _repository.AddOrUpdate(model);

            return model;
        }

        public void AddRange(params ScenarioCampaignResult[] models)
        {
            foreach (var model in models)
            {
                _repository.AddOrUpdate(model);
            }
        }

        public int Count() => _dbContext
            .Query<ScenarioCampaignResultEntity>()
            .Count();

        public void DeleteAll() => _dbContext
            .Truncate<ScenarioCampaignResultEntity>();

        public IEnumerable<ScenarioCampaignResult> GetAll() => _dbContext
            .Query<ScenarioCampaignResultEntity>()
            .GroupBy(e => e.ScenarioId)
            .Select(e => new ScenarioCampaignResult
            {
                Id = e.Key,
                Items = _mapper.Map<List<ScenarioCampaignResultItem>>(e.ToList())
            })
            .ToArray();
    }
}
