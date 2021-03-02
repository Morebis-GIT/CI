using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using xggameplan.utils.seeddata.Infrastructure;

namespace xggameplan.utils.seeddata.RavenDb
{
    public class SeedRavenDomainModelContext : RavenDomainModelContext
    {
        private readonly IRavenDbContext _dbContext;
        private readonly PageReadingOptions _pageReadingOptions;

        public SeedRavenDomainModelContext(IRavenDbContext dbContext, PageReadingOptions pageReadingOptions) : base(dbContext)
        {
            _dbContext = dbContext;
            _pageReadingOptions = pageReadingOptions ?? throw new ArgumentNullException(nameof(pageReadingOptions));
        }

        public override IEnumerable<TModel> GetAll<TModel>()
        {
            return _dbContext.Specific.GetAllWithNoTracking<TModel>(_pageReadingOptions.PageSize);
        }
    }
}
