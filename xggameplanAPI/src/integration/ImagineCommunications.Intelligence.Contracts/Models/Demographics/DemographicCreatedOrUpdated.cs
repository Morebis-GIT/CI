using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics
{
    public class DemographicCreatedOrUpdated : IDemographicCreatedOrUpdated
    {
        public int CustomId { get; set; }
        public string ExternalRef { get; }
        public string Name { get; }
        public string ShortName { get; }
        public int DisplayOrder { get; }
        public bool Gameplan { get; }

        public DemographicCreatedOrUpdated(int customId, string externalRef, string name, string shortName, int displayOrder, bool gameplan)
        {
            CustomId = customId;
            ExternalRef = externalRef;
            Name = name;
            ShortName = shortName;
            DisplayOrder = displayOrder;
            Gameplan = gameplan;
        }
    }
}
