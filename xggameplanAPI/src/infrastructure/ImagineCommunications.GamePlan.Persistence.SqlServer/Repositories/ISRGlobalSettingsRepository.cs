using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ISRGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ISRGlobalSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ISRGlobalSettingsRepository: IISRGlobalSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ISRGlobalSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public ISRGlobalSettings Get() =>
            _dbContext.Query<ISRGlobalSettingsEntity>()
                .ProjectTo<ISRGlobalSettings>(_mapper.ConfigurationProvider)
                .SingleOrDefault() ?? new ISRGlobalSettings();

        public ISRGlobalSettings Update(ISRGlobalSettings settings)
        {
            var existingSettings = _dbContext.Query<ISRGlobalSettingsEntity>().SingleOrDefault() ??
                new ISRGlobalSettingsEntity();

            _mapper.Map(settings, existingSettings);

            _dbContext.Update(existingSettings, post => post.MapTo(settings), _mapper);

            return settings;
        }
    }
}
