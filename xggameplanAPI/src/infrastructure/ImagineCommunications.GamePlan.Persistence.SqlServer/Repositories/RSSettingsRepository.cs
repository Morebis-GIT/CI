using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RSSettingsRepository : IRSSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public RSSettingsRepository(ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public RSSettings Find(string salesArea) =>
             _mapper.Map<RSSettings>(
                    RSSettingsQuery
                    .Include(x => x.SalesArea)
                    .FirstOrDefault(x => x.SalesArea.Name == salesArea),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<RSSettings> FindBySalesAreas(IEnumerable<string> salesAreas) =>
            _mapper.Map<List<RSSettings>>(
                    RSSettingsQuery
                    .Include(x => x.SalesArea)
                    .Where(x => salesAreas.Contains(x.SalesArea.Name)).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public List<RSSettings> GetAll() =>
             _mapper.Map<List<RSSettings>>(
                RSSettingsQuery.AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Add(IEnumerable<RSSettings> rsSettings) =>
            _dbContext.AddRange(_mapper.Map<RSSettingsEntity[]>(rsSettings, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(rsSettings, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        /// <summary>
        /// Add or Update method
        /// </summary>
        /// <param name="rsSettings"></param>
        public void Update(RSSettings rsSettings)
        {
            if (rsSettings == null)
            {
                throw new ArgumentNullException(nameof(rsSettings));
            }

            var entity = RSSettingsQuery
                .FirstOrDefault(x => x.Id == rsSettings.Id);

            if (entity == null)
            {
                _ = _dbContext.Add(
                    _mapper.Map<RSSettingsEntity>(rsSettings,
                        opts => opts.UseEntityCache(_salesAreaByNameCache)),
                            post => post.MapTo(
                                rsSettings,
                                opts => opts.UseEntityCache(_salesAreaByIdCache)),
                    _mapper);
            }
            else
            {
                _ = _mapper.Map(rsSettings, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity, post => post.MapTo(rsSettings, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void Delete(string salesArea)
        {
            var items = _dbContext.Query<RSSettingsEntity>()
                .Where(x => x.SalesArea.Name == salesArea)
                .ToArray();

            _dbContext.RemoveRange(items);
        }

        private IQueryable<RSSettingsEntity> RSSettingsQuery =>
            _dbContext.Query<RSSettingsEntity>()
                .Include(x => x.DemographicsSettings).ThenInclude(x => x.DeliverySettingsList)
                .Include(x => x.DefaultDeliverySettingsList);

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
