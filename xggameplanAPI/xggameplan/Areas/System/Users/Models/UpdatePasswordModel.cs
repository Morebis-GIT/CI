namespace xggameplan.Areas.System.Users.Models
{
    /// <summary>
    /// Model carrying data needed to update password.
    /// </summary>
    public class UpdatePasswordModel
    {
        /// <summary>
        /// Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// CurrentPassword.
        /// </summary>
        public string CurrentPassword { get; set; }

        /// <summary>
        /// NewPassword.
        /// </summary>
        public string NewPassword { get; set; }
    }
}
