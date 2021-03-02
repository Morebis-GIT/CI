using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RSGlobalSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RSGlobalSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RSGlobalSettingsRepository : IRSGlobalSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RSGlobalSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public RSGlobalSettings Get() =>
            _dbContext.Query<RSGlobalSettingsEntity>()
                .ProjectTo<RSGlobalSettings>(_mapper.ConfigurationProvider)
                .SingleOrDefault() ?? new RSGlobalSettings();

        public RSGlobalSettings Update(RSGlobalSettings settings)
        {
            var existingSettings = _dbContext.Query<RSGlobalSettingsEntity>()
                .SingleOrDefault() ?? new RSGlobalSettingsEntity();

            _mapper.Map(settings, existingSettings);

            _dbContext.Update(existingSettings,
                post => post.MapTo(settings), _mapper);

            return settings;
        }
    }
}
