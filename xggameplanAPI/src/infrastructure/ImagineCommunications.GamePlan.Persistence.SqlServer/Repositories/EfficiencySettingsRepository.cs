using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class EfficiencySettingsRepository : IEfficiencySettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public EfficiencySettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public EfficiencySettings GetDefault()
        {
            var count = _dbContext.Query<Entities.Tenant.EfficiencySettings>().Count();
            if (count <= 0)
            {
                throw new RepositoryException("There is no efficiency settings record");
            }

            if (count > 1)
            {
                throw new RepositoryException("There are more than one efficiency settings record");
            }

            return _mapper.Map<EfficiencySettings>(_dbContext.Query<Entities.Tenant.EfficiencySettings>().FirstOrDefault());
        }

        public EfficiencySettings UpdateDefault(EfficiencySettings settings)
        {
            var settingsToSave = settings.Id == default ?
                GetDefault().FulfillFrom(settings) : settings;
            var entity = _dbContext.Find<Entities.Tenant.EfficiencySettings>(settingsToSave.Id);
            _mapper.Map(settingsToSave, entity);
            _dbContext.Update(entity, post => post.MapTo(settingsToSave), _mapper);
            return settingsToSave;
        }
    }
}
