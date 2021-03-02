using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class PassDomainModelHandler : IDomainModelHandler<Pass>
    {
        private readonly IPassRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public PassDomainModelHandler(IPassRepository repository, ISqlServerDbContext dbContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Pass Add(Pass model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params Pass[] models) => _repository.Add(models);

        public int Count() => _dbContext.Query<PassEntity>().Count();
        
        public void DeleteAll() => _dbContext.Truncate<PassEntity>();

        public IEnumerable<Pass> GetAll() => _repository.GetAll();
    }
}
