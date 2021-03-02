using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class UserSetting : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string Name { get; set; }

        public string Value { get; set; }

    }
}
