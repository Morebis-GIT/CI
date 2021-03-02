using System.ComponentModel.DataAnnotations;

namespace xggameplan.Areas.System.Auth.Models
{
    /// <summary>
    /// Model required when obtaining an access token.
    /// </summary>
    public class GetAccessTokenModel
    {
        /// <summary>
        /// User's email which works as a login.
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// User's password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
