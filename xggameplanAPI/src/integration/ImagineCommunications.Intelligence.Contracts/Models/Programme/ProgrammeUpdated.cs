using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme
{
    public class ProgrammeUpdated : IProgrammeUpdated
    {
        public ProgrammeUpdated(string externalReference, string programmeName,
            string description, string classification, bool liveBroadcast)
        {
            ExternalReference = externalReference;
            ProgrammeName = programmeName;
            Description = description;
            Classification = classification;
            LiveBroadcast = liveBroadcast;
        }

        public string ExternalReference { get; }
        public string ProgrammeName { get; }
        public string Description { get; }
        public string Classification { get; }
        public bool LiveBroadcast { get; }
    }
}
