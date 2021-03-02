using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.core.Repository;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenUserSettingsRepository : IUserSettingsService
    {
        private readonly IUsersRepository _userRepository;

        public RavenUserSettingsRepository(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void DeleteUserSetting(int userId, string key)
        {
            var user = _userRepository.GetById(userId);
            if (user.UserSettings.ContainsKey(key))
            {
                user.UserSettings.Remove(key);
                _userRepository.Update(user);
            }
        }

        public string GetUserSetting(int userId, string key)
        {
            var user = _userRepository.GetById(userId);
            if (user.UserSettings.ContainsKey(key))
            {
                return user.UserSettings[key];
            }
            else
            {
                return null;
            }
        }

        public void SaveChanges() => _userRepository.SaveChanges();

        public void SetUserSetting(int userId, string key, string value)
        {
            var user = _userRepository.GetById(userId);
            user.UserSettings[key] = value;
            _userRepository.Update(user);
        }
    }
}
