namespace xggameplan.core.Repository
{
    public interface IUserSettingsService
    {
        /// <summary>
        /// Returns null if given user has no setting with the given key.
        /// </summary>
        string GetUserSetting(int userId, string key);

        /// <summary>
        /// Sets value of the user setting. If value with the given key already exists, it is overwritten.
        /// </summary>
        void SetUserSetting(int userId, string key, string value);

        /// <summary>
        /// Deletes saved user setting if it exists. Does nothing if the value does not exist.
        /// </summary>
        void DeleteUserSetting(int userId, string key);

        void SaveChanges();
    }
}
