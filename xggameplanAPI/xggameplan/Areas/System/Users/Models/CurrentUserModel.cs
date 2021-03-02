using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;

namespace xggameplan.Areas.System.Users.Models
{
    /// <summary>
    /// Representation of the currently signed in user.
    /// </summary>
    public class CurrentUserModel
    {
        /// <summary>
        /// User id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Name of the theme defining the design of the UI for this user.
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Where the user is located.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// What role is the user holding.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Information about tenant user belongs to.
        /// </summary>
        public CurrentUserTenantModel Tenant { get; set; }

        /// <summary>
        /// Default time zone where this user operates. Will affect the UI when user logs in for the first time.
        /// </summary>
        public string DefaultTimeZone { get; set; }
    }

    /// <summary>
    /// Represents tenant information of the current user.
    /// </summary>
    public class CurrentUserTenantModel
    {
        /// <summary>
        /// Tenant id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tenant name.
        /// </summary>
        public string Name { get; set; }
    }

    public class CurrentUserModelProfile : AutoMapper.Profile
    {
        public CurrentUserModelProfile()
        {
            CreateMap<User, CurrentUserModel>();
            CreateMap<Tenant, CurrentUserTenantModel>();
        }
    }
}
