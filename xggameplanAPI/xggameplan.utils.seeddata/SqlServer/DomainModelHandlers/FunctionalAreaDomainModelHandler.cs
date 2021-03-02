using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class FunctionalAreaDomainModelHandler : IDomainModelHandler<FunctionalArea>
    {
        private readonly IFunctionalAreaRepository _functionalAreaRepository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public FunctionalAreaDomainModelHandler(IFunctionalAreaRepository functionalAreaRepository, ISqlServerTenantDbContext dbContext)
        {
            _functionalAreaRepository = functionalAreaRepository ?? throw new ArgumentNullException(nameof(functionalAreaRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IEnumerable<FunctionalArea> GetAll() =>
            _functionalAreaRepository.GetAll();

        public FunctionalArea Add(FunctionalArea model)
        {
            _functionalAreaRepository.Add(model);
            return model;
        }

        public void AddRange(params FunctionalArea[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() =>
            _dbContext
                .Query<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas.FunctionalArea>().Count();

        public void DeleteAll()
        {
            _dbContext.Truncate<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas.FunctionalArea>();
            _dbContext.Truncate<ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas.FaultType>();
        }
    }
}
