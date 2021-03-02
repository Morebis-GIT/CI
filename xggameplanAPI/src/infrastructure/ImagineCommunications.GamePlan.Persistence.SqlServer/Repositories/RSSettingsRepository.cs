using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using RSSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSSettings.RSSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RSSettingsRepository : IRSSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RSSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public RSSettings Find(string salesArea) =>
            _dbContext.Query<RSSettingsEntity>()
                .ProjectTo<RSSettings>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.SalesArea == salesArea);

        public IEnumerable<RSSettings> FindBySalesAreas(IEnumerable<string> salesAreas) =>
            _dbContext.Query<RSSettingsEntity>()
                .Where(x => salesAreas.Contains(x.SalesArea))
                .ProjectTo<RSSettings>(_mapper.ConfigurationProvider)
                .ToList();

        public List<RSSettings> GetAll() =>
            _dbContext.Query<RSSettingsEntity>()
                .ProjectTo<RSSettings>(_mapper.ConfigurationProvider)
                .ToList();

        public void Add(IEnumerable<RSSettings> rsSettings) =>
            _dbContext.AddRange(_mapper.Map<RSSettingsEntity[]>(rsSettings),
                post => post.MapToCollection(rsSettings), _mapper);

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

            var entity = _dbContext.Query<RSSettingsEntity>()
                .Include(x => x.DemographicsSettings).ThenInclude(x => x.DeliverySettingsList)
                .Include(x => x.DefaultDeliverySettingsList)
                .FirstOrDefault(x => x.Id == rsSettings.Id);

            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<RSSettingsEntity>(rsSettings), post => post.MapTo(rsSettings), _mapper);
            }
            else
            {
                _mapper.Map(rsSettings, entity);
                _dbContext.Update(entity, post => post.MapTo(rsSettings), _mapper);
            }
        }

        public void Delete(string salesArea)
        {
            var items = _dbContext.Query<RSSettingsEntity>()
                .Where(x => x.SalesArea == salesArea)
                .ToArray();

            _dbContext.RemoveRange(items);
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
