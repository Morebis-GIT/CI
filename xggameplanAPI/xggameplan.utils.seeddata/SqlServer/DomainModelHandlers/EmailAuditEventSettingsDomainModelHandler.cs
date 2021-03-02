using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.AuditEvents;
using EmailAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.EmailAuditEventSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class EmailAuditEventSettingsDomainModelHandler : IDomainModelHandler<EmailAuditEventSettings>
    {
        private readonly IEmailAuditEventSettingsRepository _emailAuditEventSettingsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public EmailAuditEventSettingsDomainModelHandler(IEmailAuditEventSettingsRepository emailAuditEventSettingsRepository,
            ISqlServerDbContext dbContext)
        {
            _emailAuditEventSettingsRepository =
                emailAuditEventSettingsRepository ?? throw new ArgumentNullException(nameof(emailAuditEventSettingsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public EmailAuditEventSettings Add(EmailAuditEventSettings model)
        {
            AddRange(model);
            return model;
        }

        public void AddRange(params EmailAuditEventSettings[] models) => _emailAuditEventSettingsRepository.AddRange(new List<EmailAuditEventSettings>(models));

        public int Count() => _dbContext.Query<EmailAuditEventSettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<EmailAuditEventSettingsEntity>();

        public IEnumerable<EmailAuditEventSettings> GetAll() => _emailAuditEventSettingsRepository.GetAll();
    }
}
