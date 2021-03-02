using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics
{
    public class DemographicUpdated :IDemographicUpdated
    {
        public string ExternalRef { get; }
        public string Name { get; }
        public string ShortName { get; }
        public int DisplayOrder { get; }
        public bool Gameplan { get; }

        public DemographicUpdated(string externalRef, string name, string shortName, int displayOrder, bool gameplan)
        {
            ExternalRef = externalRef;
            Name = name;
            ShortName = shortName;
            DisplayOrder = displayOrder;
            Gameplan = gameplan;
        }
    }
}
