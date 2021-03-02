using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash
{
    public class ClashUpdated : IClashUpdated
    {
        public ClashUpdated(string parentExternalidentifier, string description, int exposureCount, string externalref, List<ClashDifference> differences)
        {
            ParentExternalidentifier = parentExternalidentifier;
            Description = description;
            ExposureCount = exposureCount;
            Externalref = externalref;
            Differences = differences;
        }

        public string ParentExternalidentifier { get; }

        public string Description { get; }

        public int ExposureCount { get; }

        public string Externalref { get; }

        public List<ClashDifference> Differences { get; }
    }
}
