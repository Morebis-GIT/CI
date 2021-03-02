using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    public partial class SponsorshipRestrictionFilterServiceTests
    {
        [Fact(DisplayName = "Given one sponsorship records " +
            "When the given programme name does not match the Programme " +
            "Then one sponsorship item is returned")]
        public void SponsorshipRestrictionsNotForGivenProgrammeNameTimeBandReturnsOneResults()
        {
            // Arrange
            var sponsorshipRestrictions = CreateSponsorshipWithProgrammeRestrictionLevel();
            sponsorshipRestrictions[0].RestrictionLevel = SponsorshipRestrictionLevel.TimeBand;
            foreach (var s in sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems)
            { s.ProgrammeName = "NFL"; }

            var programme = CreateProgramme();
            var filter = new SponsorshipRestrictionFilterService(sponsorshipRestrictions);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle();
            _ = result[0].SponsoredItems.Should().ContainSingle();
            foreach (var s in result[0].SponsoredItems[0].SponsorshipItems)
            {
                _ = s.ProgrammeName.Should().Be("NFL");
            }
        }

        [Fact(DisplayName = "Given two sponsorship item records " +
          "When the given programme name does not match the Programme and one does " +
          "Then one sponsorship item is returned")]
        public void TwoSponsorshipRestrictionsNotForGivenProgrammeNameTimeBandReturnsOneResults()
        {
            // Arrange
            var sponsorshipRestrictions = CreateSponsorshipWithProgrammeRestrictionLevel();
            sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems.Last().ProgrammeName = "NFL";

            var programme = CreateProgramme();
            var filter = new SponsorshipRestrictionFilterService(sponsorshipRestrictions);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle();
            _ = result[0].SponsoredItems.Should().ContainSingle();
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().ContainSingle();
        }
    }
}
