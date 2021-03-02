using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class TenantsDomainModelHandler : IDomainModelHandler<Tenant>
    {
        private readonly ITenantsRepository _tenantsRepository;
        private readonly ISqlServerMasterDbContext _dbContext;

        public TenantsDomainModelHandler(ITenantsRepository tenantRepository, ISqlServerMasterDbContext dbContext)
        {
            _tenantsRepository = tenantRepository;
            _dbContext = dbContext;
        }

        public Tenant Add(Tenant model)
        {
            _tenantsRepository.Add(model);
            return model;
        }

        public void AddRange(params Tenant[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<MasterEntities.Tenant>().Count();

        public void DeleteAll() => _dbContext.Truncate<MasterEntities.Tenant>();

        public IEnumerable<Tenant> GetAll() => _tenantsRepository.GetAll();
    }
}
