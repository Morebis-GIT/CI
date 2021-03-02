using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using TenantSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.TenantSettings.TenantSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class TenantSettingsRepository : ITenantSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;
        private TenantSettings _cachedTenantSettings;

        public TenantSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public TenantSettings Get()
        {
            if (_cachedTenantSettings == null)
            {
                _cachedTenantSettings = _mapper.Map<TenantSettings>(_dbContext.Query<TenantSettingsEntity>()
                    .Include(p => p.Features)
                    .Include(p => p.RunEventSettings)
                    .Include(p => p.WebhookSettings)
                    .Include(p => p.RunRestrictions).FirstOrDefault());
            }
            return _cachedTenantSettings;
        }

        public void AddOrUpdate(TenantSettings tenantSettings)
        {
            var currentEntity = _dbContext.Query<TenantSettingsEntity>()
                .Include(x => x.RunEventSettings)
                .Include(x => x.Features)
                .Include(x => x.WebhookSettings)
                .FirstOrDefault();

            if (currentEntity == null)
            {
                _dbContext.Add(_mapper.Map<TenantSettingsEntity>(tenantSettings),
                    post => post.MapTo(tenantSettings), _mapper);
            }
            else
            {
                _mapper.Map(tenantSettings, currentEntity);
                _dbContext.Update(currentEntity, post => post.MapTo(tenantSettings), _mapper);
            }

            _cachedTenantSettings = tenantSettings;
        }

        public void SaveChanges() => _dbContext.SaveChanges();

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
