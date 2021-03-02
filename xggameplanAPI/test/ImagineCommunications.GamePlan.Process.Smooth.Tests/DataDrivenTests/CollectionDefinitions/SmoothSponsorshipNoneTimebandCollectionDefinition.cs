﻿using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [CollectionDefinition("Smooth - Sponsorship - None - Timeband")]
    public class SmoothSponsorshipNoneTimebandCollectionDefinition
        : ICollectionFixture<SmoothFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        //
        // See https://xunit.net/docs/shared-context
    }
}
