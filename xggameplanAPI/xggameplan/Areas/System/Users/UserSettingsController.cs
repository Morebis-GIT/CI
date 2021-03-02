using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using xggameplan.Areas.System.Users.Models;
using xggameplan.core.Repository;
using xggameplan.Extensions;
using xggameplan.Filters;

namespace xggameplan.Areas.System.Users
{
    /// <summary>
    /// Provides API methods for working with user's settings. User settings is a key-value pair stored per user.
    /// The user settings can be used to persist user's UI preferences, such as default selections, etc. By default, settings are empty.
    /// </summary>
    [RoutePrefix("usersettings")]
    [AuthorizeRequest("UserSettings")]
    public class UserSettingsController : ApiController
    {
        private readonly IUserSettingsService _userSettingsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UserSettingsController(IUserSettingsService userSettingsRepository)
        {
            _userSettingsService = userSettingsRepository;
        }

        /// <summary>
        /// Returns value of the setting with specified key belonging to the user authenticated by provided token. Returns 404 if settings with the specified key does not exist.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <returns>Setting value</returns>
        [Route("{key}")]
        [ResponseType(typeof(UserSettingsModel))]
        public IHttpActionResult Get(string key)
        {
            string value = _userSettingsService.GetUserSetting(GetCurrentUserId(), key);

            if (value == null)
            {
                return Ok(new UserSettingsModel { Value = null });
            }

            return Ok(new UserSettingsModel
            {
                Value = value,
            });
        }

        [Route("{key}")]
        [ResponseType(typeof(UserSettingsModel))]
        public IHttpActionResult Post(string key)
        {
            string value = _userSettingsService.GetUserSetting(GetCurrentUserId(), key);

            if (value == null)
            {
                return Ok(new UserSettingsModel { Value = null });
            }

            return Ok(new UserSettingsModel
            {
                Value = value,
            });
        }

        /// <summary>
        /// Sets value of the settings key. If value with the given key already exists, it is overwritten.
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">Value to be stored</param>
        [Route("{key}")]
        public void Put(string key, [FromBody] UserSettingsModel value) {
            _userSettingsService.SetUserSetting(GetCurrentUserId(), key, value.Value);
            _userSettingsService.SaveChanges();
        }

        /// <summary>
        /// Deletes user setting with the specified key. Does nothing if setting doesn't exist.
        /// </summary>
        /// <param name="key">Settings key</param>
        [Route("{key}")]
        public void Delete(string key)
        {
            _userSettingsService.DeleteUserSetting(GetCurrentUserId(), key);
            _userSettingsService.SaveChanges();
        }

        private int GetCurrentUserId() => HttpContext.Current.GetCurrentUser().Id;
    }
}
