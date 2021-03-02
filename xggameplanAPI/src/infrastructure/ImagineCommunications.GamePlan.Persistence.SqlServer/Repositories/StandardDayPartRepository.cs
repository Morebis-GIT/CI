using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class StandardDayPartRepository : IStandardDayPartRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public StandardDayPartRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<StandardDayPart> GetAll() =>
            _dbContext.Query<Entities.Tenant.DayParts.StandardDayPart>()
                .ProjectTo<StandardDayPart>(_mapper.ConfigurationProvider)
                .ToList();

        public StandardDayPart Get(int id) =>
            _mapper.Map<StandardDayPart>(_dbContext.Find<Entities.Tenant.DayParts.StandardDayPart>(new object[] {id},
                find => find.IncludeCollection(x => x.Timeslices)));

        public void AddRange(IEnumerable<StandardDayPart> dayParts) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.DayParts.StandardDayPart[]>(dayParts),
                post => post.MapToCollection(dayParts), _mapper);

        public IEnumerable<StandardDayPart> FindByExternal(List<int> externalIds) =>
            _dbContext.Query<Entities.Tenant.DayParts.StandardDayPart>()
                .Where(e => externalIds.Contains(e.DayPartId))
                .ProjectTo<StandardDayPart>(_mapper.ConfigurationProvider)
                .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.DayParts.StandardDayPart>();
    }
}
