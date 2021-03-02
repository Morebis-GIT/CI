using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using xggameplan.specification.tests.Interfaces;
using FunctionalArea = ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects.FunctionalArea;
using FunctionalAreaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas.FunctionalArea;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    public class FunctionalAreaDomainModelHandler : SimpleDomainModelMappingHandler<FunctionalAreaEntity, FunctionalArea>
    {
        private readonly ITenantDbContext _dbContext;

        public FunctionalAreaDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override IEnumerable<FunctionalArea> GetAll() => throw new NotSupportedException();
    }
}
