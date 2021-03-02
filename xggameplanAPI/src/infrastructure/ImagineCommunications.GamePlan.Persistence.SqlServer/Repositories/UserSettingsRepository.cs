using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using xggameplan.core.Repository;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class UserSettingsRepository : IUserSettingsService
    {
        private readonly ISqlServerMasterDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserSettingsRepository(ISqlServerMasterDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public string GetUserSetting(int userId, string key) =>
            _dbContext.Query<UserSetting>()
                .SingleOrDefault(e => e.UserId == userId && e.Name == key )?.Value;

        public void SetUserSetting(int userId, string key, string value)
        {
            var setting = _dbContext.Query<UserSetting>().FirstOrDefault(e => e.UserId == userId && e.Name == key);
            if (setting != null)
            {
                setting.Value = value;
                _dbContext.Update(setting, post => post.MapTo(setting), _mapper);
            }
            else
            {
                _dbContext.Add(new UserSetting() { Name = key, UserId = userId, Value = value });
            }
        }
        public void DeleteUserSetting(int userId, string key)
        {
            var settingToDelete = _dbContext.Query<UserSetting>().FirstOrDefault(e => e.UserId == userId && e.Name == key);
            if (settingToDelete != null)
            {
                _dbContext.Remove(settingToDelete);
            }
        }
    }
}
