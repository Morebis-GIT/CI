using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.AuditEvents;
using MSTeamsAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsAuditEventSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class MSTeamsAuditEventSettingsRepository : IMSTeamsAuditEventSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public MSTeamsAuditEventSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Insert(List<MSTeamsAuditEventSettings> items) =>
            _dbContext.AddRange(_mapper.Map<MSTeamsAuditEventSettingsEntity[]>(items),
                post => post.MapToCollection(items), _mapper);

        public List<MSTeamsAuditEventSettings> GetAll() =>
            _dbContext.Query<MSTeamsAuditEventSettingsEntity>()
                .ProjectTo<MSTeamsAuditEventSettings>(_mapper.ConfigurationProvider)
                .ToList();

        public void DeleteAll() =>
            _dbContext.Truncate<MSTeamsAuditEventSettingsEntity>();

        public void DeleteByID(int id)
        {
            var item = _dbContext.Query<MSTeamsAuditEventSettingsEntity>()
                .FirstOrDefault(x => x.EventTypeId == id);

            if (item != null)
            {
                _dbContext.Remove(item);
            }
        }

        public void Update(MSTeamsAuditEventSettings item)
        {
            var entity = _dbContext.Query<MSTeamsAuditEventSettingsEntity>()
                .FirstOrDefault(e => e.EventTypeId == item.EventTypeId);

            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
