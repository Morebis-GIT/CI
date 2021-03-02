using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class LibrarySalesAreaPassPrioritiesRepositoryAdapter : RepositoryTestAdapter<LibrarySalesAreaPassPriority, ILibrarySalesAreaPassPrioritiesRepository, Guid>
    {
        public LibrarySalesAreaPassPrioritiesRepositoryAdapter(IScenarioDbContext dbContext, ILibrarySalesAreaPassPrioritiesRepository repository) : base(dbContext, repository)
        {
        }

        protected override LibrarySalesAreaPassPriority Add(LibrarySalesAreaPassPriority model)
        {
            Repository.AddAsync(model).Wait();
            return model;
        }

        protected override IEnumerable<LibrarySalesAreaPassPriority> AddRange(params LibrarySalesAreaPassPriority[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(Guid id) =>
            Repository.Delete(id).Wait();

        protected override IEnumerable<LibrarySalesAreaPassPriority> GetAll() =>
            Repository.GetAllAsync().Result;

        protected override LibrarySalesAreaPassPriority GetById(Guid id) =>
            Repository.GetAsync(id).Result;

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override LibrarySalesAreaPassPriority Update(LibrarySalesAreaPassPriority model)
        {
            _ = Repository.UpdateAsync(model);
            return model;
        }
    }
}
