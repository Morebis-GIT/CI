using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using ISRSettings = ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects.ISRSettings;
using ISRSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRSettings.ISRSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ISRSettingsRepository : IISRSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public ISRSettingsRepository(
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

        public ISRSettings Find(string salesArea) =>
            _mapper.Map<ISRSettings>(
                _dbContext.Query<ISRSettingsEntity>()
                    .Include(x => x.SalesArea).Include(x => x.DemographicsSettings)
                    .FirstOrDefault(x => x.SalesArea.Name == salesArea),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<ISRSettings> FindBySalesAreas(IEnumerable<string> salesAreas) =>
            _mapper.Map<List<ISRSettings>>(
                _dbContext.Query<ISRSettingsEntity>()
                    .Include(x => x.DemographicsSettings)
                    .Where(x => salesAreas.Contains(x.SalesArea.Name)),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public List<ISRSettings> GetAll() =>
            _mapper.Map<List<ISRSettings>>(
                _dbContext.Query<ISRSettingsEntity>()
                    .Include(x => x.DemographicsSettings)
                , opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Add(IEnumerable<ISRSettings> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var entities = _mapper
                .Map<IEnumerable<ISRSettingsEntity>>(items, opts => opts.UseEntityCache(_salesAreaByNameCache))
                .ToArray();

            _dbContext.AddRange(entities,
                x => x.MapToCollection(items, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
        }

        public void Update(ISRSettings isrSettings)
        {
            if (isrSettings == null)
            {
                throw new ArgumentNullException(nameof(isrSettings));
            }

            var entity = _dbContext.Find<ISRSettingsEntity>(new object[] { isrSettings.Id },
                post =>
                {
                    _ = post.IncludeCollection(e => e.DemographicsSettings);
                });

            if (entity != null)
            {
                _ = _mapper.Map(isrSettings, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity,
                    post => post.MapTo(isrSettings, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void Delete(string salesArea)
        {
            var items = _dbContext.Query<ISRSettingsEntity>()
                .Where(x => x.SalesArea.Name == salesArea)
                .ToArray();

            if (items.Any() == false)
            {
                throw new InvalidOperationException("Sequence contains no elements");
            }

            _dbContext.RemoveRange(items);
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
