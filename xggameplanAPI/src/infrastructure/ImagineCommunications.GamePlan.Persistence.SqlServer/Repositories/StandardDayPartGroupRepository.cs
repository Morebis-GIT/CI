using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using StandardDayPartGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPartGroup;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class StandardDayPartGroupRepository : IStandardDayPartGroupRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public StandardDayPartGroupRepository(
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

        public IEnumerable<StandardDayPartGroup> GetAll() =>
            _mapper.Map<List<StandardDayPartGroup>>(
                _dbContext.Query<StandardDayPartGroupEntity>()
                .Include(x => x.Splits).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public StandardDayPartGroup Get(int id) =>
            _mapper.Map<StandardDayPartGroup>(_dbContext.Find<StandardDayPartGroupEntity>(new object[] { id },
                find => find.IncludeCollection(x => x.Splits)),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void AddRange(IEnumerable<StandardDayPartGroup> dayPartGroups) =>
            _dbContext.AddRange(_mapper.Map<StandardDayPartGroupEntity[]>(dayPartGroups,
                    opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(dayPartGroups,
                    opts => opts.UseEntityCache(_salesAreaByIdCache)),
                _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<StandardDayPartGroupEntity>();
    }
}
