using System;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenTenantSettingsRepository : ITenantSettingsRepository
    {
        private readonly IDocumentSession _session;
        private readonly IMapper _mapper;

        public RavenTenantSettingsRepository(IDocumentSession session, IMapper mapper)
        {
            _session = session;
            _mapper = mapper;
        }

        public TenantSettings Get()
        {
            try
            {
                return _session.Query<TenantSettings>().FirstOrDefault();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("There is a configuration problem with the tenant settings.", ex);
            }
        }

        public void AddOrUpdate(TenantSettings tenantSettings)
        {
            var currentSettings = Get();
            if (tenantSettings == currentSettings)
            {
                return;
            }

            if (currentSettings != null)
            {
                _mapper.Map(tenantSettings, currentSettings);
                _session.Store(currentSettings);
            }
            else
            {
                _session.Store(tenantSettings);
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public Guid GetDefaultSalesAreaPassPriorityId()
        {
            var result = Get();
            return result?.DefaultSalesAreaPassPriorityId ?? Guid.Empty;
        }

        public Guid GetDefaultScenarioId()
        {
            var result = Get();
            return result?.DefaultScenarioId ?? Guid.Empty;
        }

        public DayOfWeek GetStartDayOfWeek()
        {
            var result = Get();

            return result is null ? DayOfWeek.Monday : result.StartDayOfWeek;
        }

        public void Truncate() => throw new NotImplementedException();
    }
}
