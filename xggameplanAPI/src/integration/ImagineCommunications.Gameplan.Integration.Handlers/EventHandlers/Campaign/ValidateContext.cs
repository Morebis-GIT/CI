using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign
{
    internal sealed class ValidateContext
    {
        public IReadOnlyCollection<string> ExistingSalesAreas { get; set; }
        public ImmutableHashSet<string> ExistingDemographics { get; set; }
        public ImmutableHashSet<string> Products { get; set; }
        public ImmutableHashSet<string> ProgrammeCategories { get; set; }
        public ImmutableHashSet<int> BookingPositionGroups { get; set; }
        public ImmutableHashSet<string> BreakTypes { get; set; }
    }
}
