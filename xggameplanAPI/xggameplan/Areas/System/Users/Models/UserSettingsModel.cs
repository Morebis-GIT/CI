using System.ComponentModel.DataAnnotations;

namespace xggameplan.Areas.System.Users.Models
{
    /// <summary>
    /// Represents value of the settings stored by a user under some key.
    /// </summary>
    public class UserSettingsModel
    {
        /// <summary>
        /// Settings value.
        /// </summary>
        [Required]
        public string Value { get; set; }
    }
}
