using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionDefinitions
{
    [CollectionDefinition("Smooth - Sponsorship - Percentage - Programme - All competitors")]
    public class SmoothSponsorshipPercentageProgrammeAllCompetitorsCollectionDefinition
        : ICollectionFixture<SmoothFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        //
        // See https://xunit.net/docs/shared-context
    }
}
