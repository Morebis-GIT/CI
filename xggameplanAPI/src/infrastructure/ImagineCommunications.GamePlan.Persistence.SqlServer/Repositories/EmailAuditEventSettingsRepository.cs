using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.AuditEvents;
using EmailAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.EmailAuditEventSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class EmailAuditEventSettingsRepository : IEmailAuditEventSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public EmailAuditEventSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddRange(IEnumerable<EmailAuditEventSettings> items) =>
            _dbContext.AddRange(_mapper.Map<EmailAuditEventSettingsEntity[]>(items),
                post => post.MapToCollection(items), _mapper);

        public List<EmailAuditEventSettings> GetAll() => _mapper.Map<List<EmailAuditEventSettings>>(
            _dbContext.Query<EmailAuditEventSettingsEntity>().Include(p => p.NotificationSettings));

        public void Truncate() => _dbContext.Truncate<EmailAuditEventSettingsEntity>();

        public void Delete(int eventTypeid)
        {
            var items = _dbContext.Query<EmailAuditEventSettingsEntity>()
                .Where(x => x.EventTypeId == eventTypeid)
                .ToArray();

            _dbContext.RemoveRange(items);
        }

        public void Update(EmailAuditEventSettings item)
        {
            var entity = _dbContext.Query<EmailAuditEventSettingsEntity>()
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
