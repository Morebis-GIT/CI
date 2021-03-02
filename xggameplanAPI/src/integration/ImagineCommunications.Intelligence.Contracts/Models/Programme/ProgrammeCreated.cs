using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme
{
    public class ProgrammeCreated : IProgrammeCreated
    {
        public ProgrammeCreated(string salesArea, string externalReference, string programmeName, string description, string classification, bool liveBroadcast,
            DateTime startDateTime, TimeSpan duration, IEnumerable<string> programmeCategories, ProgrammeEpisode episode)
        {
            SalesArea = salesArea;
            ExternalReference = externalReference;
            ProgrammeName = programmeName;
            Description = description;
            Classification = classification;
            LiveBroadcast = liveBroadcast;
            StartDateTime = startDateTime;
            Duration = duration;
            ProgrammeCategories = programmeCategories;
            Episode = episode;
        }

        public string SalesArea { get; }

        public string ExternalReference { get; }

        public string ProgrammeName { get; }

        public string Description { get; }

        public string Classification { get; }

        public bool LiveBroadcast { get; }

        public DateTime StartDateTime { get; }

        public TimeSpan Duration { get; }

        public IEnumerable<string> ProgrammeCategories { get; }

        public ProgrammeEpisode Episode  { get; }
    }
}
