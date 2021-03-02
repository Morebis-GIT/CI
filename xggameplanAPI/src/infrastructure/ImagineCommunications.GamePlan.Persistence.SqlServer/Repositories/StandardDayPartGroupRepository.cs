using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using StandardDayPartGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts.StandardDayPartGroup;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class StandardDayPartGroupRepository : IStandardDayPartGroupRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public StandardDayPartGroupRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<StandardDayPartGroup> GetAll() =>
            _dbContext.Query<StandardDayPartGroupEntity>()
                .ProjectTo<StandardDayPartGroup>(_mapper.ConfigurationProvider)
                .ToList();

        public StandardDayPartGroup Get(int id) =>
            _mapper.Map<StandardDayPartGroup>(_dbContext.Find<StandardDayPartGroupEntity>(new object[] { id },
                find => find.IncludeCollection(x => x.Splits)));

        public void AddRange(IEnumerable<StandardDayPartGroup> dayPartGroups) =>
            _dbContext.AddRange(_mapper.Map<StandardDayPartGroupEntity[]>(dayPartGroups),
                post => post.MapToCollection(dayPartGroups), _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<StandardDayPartGroupEntity>();
    }
}
