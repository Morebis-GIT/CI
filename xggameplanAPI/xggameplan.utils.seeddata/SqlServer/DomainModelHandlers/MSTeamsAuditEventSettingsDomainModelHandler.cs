using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.AuditEvents;
using MSTeamsAuditEventSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.MSTeamsAuditEventSettings.MSTeamsAuditEventSettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class MSTeamsAuditEventSettingsDomainModelHandler : IDomainModelHandler<MSTeamsAuditEventSettings>
    {
        private readonly IMSTeamsAuditEventSettingsRepository _msTeamsAuditEventSettingsRepository;
        private readonly ISqlServerDbContext _dbContext;

        public MSTeamsAuditEventSettingsDomainModelHandler(IMSTeamsAuditEventSettingsRepository msTeamsAuditEventSettingsRepository,
            ISqlServerDbContext dbContext)
        {
            _msTeamsAuditEventSettingsRepository =
                msTeamsAuditEventSettingsRepository ?? throw new ArgumentNullException(nameof(msTeamsAuditEventSettingsRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public MSTeamsAuditEventSettings Add(MSTeamsAuditEventSettings model)
        {
            AddRange(model);
            return model;
        }

        public void AddRange(params MSTeamsAuditEventSettings[] models) => _msTeamsAuditEventSettingsRepository.Insert(new List<MSTeamsAuditEventSettings>(models));

        public int Count() => _dbContext.Query<MSTeamsAuditEventSettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<MSTeamsAuditEventSettingsEntity>();

        public IEnumerable<MSTeamsAuditEventSettings> GetAll() => _msTeamsAuditEventSettingsRepository.GetAll();
    }
}
