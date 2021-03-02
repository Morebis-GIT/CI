using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas
{
    public class FaultTypeDescription: IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string LanguageAbbreviation { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }

        public int FaultTypeId { get; set; }
    }
}
