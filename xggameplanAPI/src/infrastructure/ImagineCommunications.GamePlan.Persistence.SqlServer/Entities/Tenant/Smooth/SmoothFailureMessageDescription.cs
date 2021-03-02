using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth
{
    public class SmoothFailureMessageDescription : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SmoothFailureMessageId { get; set; }
        public string LanguageAbbreviation { get; set; }
        public string Description { get; set; }
    }
}
