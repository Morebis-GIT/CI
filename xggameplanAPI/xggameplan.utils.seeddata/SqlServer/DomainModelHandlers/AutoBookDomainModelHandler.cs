using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutoBookEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.AutoBook;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AutoBookDomainModelHandler : IDomainModelHandler<AutoBook>
    {
        private readonly IAutoBookRepository _autoBookRepository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public AutoBookDomainModelHandler(IAutoBookRepository autoBookRepository, ISqlServerTenantDbContext dbContext)
        {
            _autoBookRepository = autoBookRepository;
            _dbContext = dbContext;
        }

        public AutoBook Add(AutoBook model)
        {
            _autoBookRepository.Add(model);
            return model;
        }

        public void AddRange(params AutoBook[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<AutoBookEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<AutoBookEntity>();

        public IEnumerable<AutoBook> GetAll() => _autoBookRepository.GetAll();
    }
}
