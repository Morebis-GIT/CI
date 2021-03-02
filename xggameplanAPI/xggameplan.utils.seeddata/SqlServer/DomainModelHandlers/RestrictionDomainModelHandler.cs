using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RestrictionDomainModelHandler : IDomainModelHandler<Restriction>
    {
        private readonly IRestrictionRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public RestrictionDomainModelHandler(IRestrictionRepository repository, ISqlServerDbContext dbContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Restriction Add(Restriction model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params Restriction[] models) => _repository.Add(models);

        public int Count() => _dbContext.Query<RestrictionEntity>().Count();
        
        public void DeleteAll() => _repository.Truncate();

        public IEnumerable<Restriction> GetAll() => _repository.GetAll();
    }
}
