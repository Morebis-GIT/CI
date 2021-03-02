using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Trait("Smooth", "Add spot to break")]
    public class AddSpotsToBreakTests
        : TestBase
    {
        public AddSpotsToBreakTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact(DisplayName = "Adding spot to a break flags the spot as used")]
        public void AddingSpotToBreakFlagsSpotAsUsed()
        {
            // Arrange
            var smoothBreak = new SmoothBreak(
                Fixture.Create<Break>(),
                position: 1);

            var spot = Fixture.Build<Spot>()
                .Without(p => p.MultipartSpot)
                .Create();

            var spots = new List<Spot> { spot };

            var spotInfos = new Dictionary<Guid, SpotInfo>
            {
                [spot.Uid] = Fixture.Create<SpotInfo>()
            };

            var spotIdsUsed = new HashSet<Guid>();

            var mockSponsorshipRestrictionService = new Mock<SponsorshipRestrictionService>(
                new Mock<IReadOnlyList<SponsorshipRestrictionFilterResults>>().Object,
                new Mock<ISmoothSponsorshipTimelineManager>().Object,
                new Mock<IReadOnlyDictionary<Guid, SpotInfo>>().Object,
                new Mock<Action<string, Exception>>().Object
                );

            mockSponsorshipRestrictionService.Setup(s =>
                s.TriggerRecalculationOfAllowedRestrictionLimits(
                    It.IsAny<SpotAction>(),
                    It.IsAny<Spot>(),
                    It.IsAny<Break>()))
                .Verifiable();

            // Act
            _ = Act(() =>
            {
                return SpotPlacementService.AddBookedSpotsToBreak(
                        smoothBreak,
                        spots,
                        spotInfos,
                        spotIdsUsed,
                        mockSponsorshipRestrictionService.Object
                    );
            });

            // Assert
            _ = spotIdsUsed.Should().Contain(spot.Uid, becauseArgs: null);
        }
    }
}
