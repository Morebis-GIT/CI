using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutopilotSettingsRepository : IAutopilotSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AutopilotSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(AutopilotSettings item) => _dbContext.Add(_mapper.Map<Entities.Tenant.AutopilotSettings>(item),
            post => post.MapTo(item), _mapper);

        public void Update(AutopilotSettings item)
        {
            var entity = _dbContext.Find<Entities.Tenant.AutopilotSettings>(item.Id);
            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.AutopilotSettings>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public AutopilotSettings Get(int id) => _mapper.Map<AutopilotSettings>(_dbContext.Find<Entities.Tenant.AutopilotSettings>(id));

        public IEnumerable<AutopilotSettings> GetAll()
        {
            return _dbContext
               .Query<Entities.Tenant.AutopilotSettings>()
               .ProjectTo<AutopilotSettings>(_mapper.ConfigurationProvider)
               .ToList();
        }

        public AutopilotSettings GetDefault()
        {
            var entity = _dbContext
              .Query<Entities.Tenant.AutopilotSettings>()
              .FirstOrDefault();
            return _mapper.Map<AutopilotSettings>(entity);
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
