using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class User : IIdentityPrimaryKey, IPreviewable
    {
        public int Id { get ; set ; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public PreviewFile Preview { get; set; }

        public string ThemeName { get; set; }

        public string Location { get; set; }

        public string Role { get; set; }

        public int TenantId { get; set; }

        public string DefaultTimeZone { get; set; }

        public List<UserSetting> UserSettings { get;  set; }
    }
}
