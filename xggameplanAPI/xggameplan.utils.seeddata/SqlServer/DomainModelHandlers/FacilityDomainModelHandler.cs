using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using FacilityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Facility;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class FacilityDomainModelHandler : IDomainModelHandler<Facility>
    {
        private readonly IFacilityRepository _repository;
        private readonly ISqlServerDbContext _dbContext;

        public FacilityDomainModelHandler(
            IFacilityRepository repository,
            ISqlServerDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public Facility Add(Facility model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params Facility[] models)
        {
            foreach (var model in models)
            {
                _repository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<FacilityEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<FacilityEntity>();

        public IEnumerable<Facility> GetAll() => _repository.GetAll();
    }
}
