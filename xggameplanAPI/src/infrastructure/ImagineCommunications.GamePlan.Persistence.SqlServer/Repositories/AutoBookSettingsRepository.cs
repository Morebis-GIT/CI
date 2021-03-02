using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using System.Linq;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AutoBookSettingsRepository : IAutoBookSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;
        private Entities.Tenant.AutoBookApi.AutoBookSettings _cachedAutoBookSettings;

        protected virtual Entities.Tenant.AutoBookApi.AutoBookSettings GetEntity()
        {
            if (_cachedAutoBookSettings == null)
            {
                _cachedAutoBookSettings = _dbContext
                    .Query<Entities.Tenant.AutoBookApi.AutoBookSettings>().FirstOrDefault();
            }

            return _cachedAutoBookSettings;
        }

        public AutoBookSettingsRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddOrUpdate(AutoBookSettings autoBookSettings)
        {
            var currentEntity = GetEntity();
            if (currentEntity == null)
            {
                currentEntity = _mapper.Map<Entities.Tenant.AutoBookApi.AutoBookSettings>(autoBookSettings);
                _dbContext.Add(currentEntity, post => post.MapTo(autoBookSettings), _mapper);
            }
            else
            {
                _mapper.Map(autoBookSettings, currentEntity);
                _dbContext.Update(currentEntity, post => post.MapTo(autoBookSettings), _mapper);
            }

            _cachedAutoBookSettings = currentEntity;
        }

        public AutoBookSettings Get() => _mapper.Map<AutoBookSettings>(GetEntity());

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
            _cachedAutoBookSettings = null;
        }
    }
}
