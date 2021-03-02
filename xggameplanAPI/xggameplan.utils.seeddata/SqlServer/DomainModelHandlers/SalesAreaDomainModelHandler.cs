using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using SalesAreaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesArea;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class SalesAreaDomainModelHandler : IDomainModelHandler<SalesArea>
    {
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ISqlServerDbContext _dbContext;

        public SalesAreaDomainModelHandler(ISalesAreaRepository salesAreaRepository, ISqlServerDbContext dbContext)
        {
            _salesAreaRepository = salesAreaRepository ?? throw new ArgumentNullException(nameof(salesAreaRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public SalesArea Add(SalesArea model)
        {
            _salesAreaRepository.Add(model);
            return model;
        }

        public void AddRange(params SalesArea[] models)
        {
            foreach (var model in models)
            {
                _salesAreaRepository.Add(model);
            }
        }

        public int Count() => _salesAreaRepository.CountAll;

        public void DeleteAll() => _dbContext.Truncate<SalesAreaEntity>();

        public IEnumerable<SalesArea> GetAll() => _salesAreaRepository.GetAll();
    }
}
