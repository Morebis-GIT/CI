namespace xggameplan.Areas.System.Users.Models
{
    public class CreateUserModel
    {
        /// <summary>
        /// First name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Email which also works as a login.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        //public string Password { get; set; }

        /// <summary>
        /// Theme Name
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Tenant Id
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public string Region { get; set; }
    }
}
