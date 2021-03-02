using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using LibrarySalesAreaPassPriorityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.LibrarySalesAreaPassPriority;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class LibrarySalesAreaPassPrioritiesDomainModelHandler : IDomainModelHandler<LibrarySalesAreaPassPriority>
    {
        private readonly ILibrarySalesAreaPassPrioritiesRepository _librarySalesAreaPassPrioritiesRepository;
        private readonly ISqlServerDbContext _dbContext;

        public LibrarySalesAreaPassPrioritiesDomainModelHandler(
            ILibrarySalesAreaPassPrioritiesRepository librarySalesAreaPassPrioritiesRepository,
            ISqlServerDbContext dbContext)
        {
            _librarySalesAreaPassPrioritiesRepository = librarySalesAreaPassPrioritiesRepository ??
                                                        throw new ArgumentNullException(
                                                            nameof(librarySalesAreaPassPrioritiesRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public LibrarySalesAreaPassPriority Add(LibrarySalesAreaPassPriority model)
        {
            _librarySalesAreaPassPrioritiesRepository.AddAsync(model).Wait();
            return model;
        }

        public void AddRange(params LibrarySalesAreaPassPriority[] models)
        {
            foreach (var model in models)
            {
                _librarySalesAreaPassPrioritiesRepository.AddAsync(model).Wait();
            }
        }

        public int Count() => _dbContext.Query<LibrarySalesAreaPassPriorityEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<LibrarySalesAreaPassPriorityEntity>();

        public IEnumerable<LibrarySalesAreaPassPriority> GetAll() =>
            _librarySalesAreaPassPrioritiesRepository.GetAllAsync().Result;
    }
}
