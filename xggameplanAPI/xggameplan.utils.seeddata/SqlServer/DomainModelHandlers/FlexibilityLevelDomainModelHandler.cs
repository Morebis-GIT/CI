using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using FlexibilityLevelEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FlexibilityLevel;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class FlexibilityLevelDomainModelHandler : IDomainModelHandler<FlexibilityLevel>
    {
        private readonly IFlexibilityLevelRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public FlexibilityLevelDomainModelHandler(IFlexibilityLevelRepository flexibilityLevelRepository, ISqlServerDbContext dbContext)
        {
            _repository = flexibilityLevelRepository ?? throw new ArgumentNullException(nameof(flexibilityLevelRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public FlexibilityLevel Add(FlexibilityLevel model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params FlexibilityLevel[] models)
        {
            foreach (var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<FlexibilityLevelEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<FlexibilityLevelEntity>();

        public IEnumerable<FlexibilityLevel> GetAll() => _repository.GetAll();
    }
}
