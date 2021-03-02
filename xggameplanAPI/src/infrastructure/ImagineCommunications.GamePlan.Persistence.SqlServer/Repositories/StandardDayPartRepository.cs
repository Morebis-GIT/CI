using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using StandardDayPartEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPart;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class StandardDayPartRepository : IStandardDayPartRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public StandardDayPartRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public IEnumerable<StandardDayPart> GetAll() =>
            _mapper.Map<List<StandardDayPart>>(
                StandardDayPartQuery().AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public StandardDayPart Get(int id) =>
            _mapper.Map<StandardDayPart>(_dbContext.Find<StandardDayPartEntity>(new object[] { id },
                find => find.IncludeCollection(x => x.Timeslices)),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void AddRange(IEnumerable<StandardDayPart> dayParts) =>
            _dbContext.AddRange(_mapper.Map<StandardDayPartEntity[]>(dayParts,
                    opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(dayParts,
                    opts => opts.UseEntityCache(_salesAreaByIdCache)),
                _mapper);

        public IEnumerable<StandardDayPart> FindByExternal(List<int> externalIds) =>
            _mapper.Map<List<StandardDayPart>>(
                StandardDayPartQuery().Where(e => externalIds.Contains(e.DayPartId)).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<StandardDayPartEntity>();

        protected IQueryable<StandardDayPartEntity> StandardDayPartQuery() =>
            _dbContext.Query<StandardDayPartEntity>()
            .Include(x => x.Timeslices);
    }
}
