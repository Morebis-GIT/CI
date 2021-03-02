using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    public partial class SponsorshipRestrictionFilterServiceTests
    {
        [Theory(DisplayName = "Given one sponsorship record " +
            "When the programme start date time falls outside the start/end time " +
                "or day of week is not in the day part" +
            "Then no sponsorship item is returned")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-21T15:40:00")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-22T06:30:00")]
        [InlineData("2020-12-01T00:00:00", "2020-12-31T00:00:00", "2020-04-21T06:30:00")]
        [InlineData("2020-01-01T00:00:00", "2020-01-31T00:00:00", "2020-04-21T06:30:00")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-25T06:30:00")]
        [InlineData("2020-04-26T00:00:00", "2020-04-20T00:00:00", "2020-04-25T06:30:00")]
        public void OneSponsorshipRestrictions_ForGivenDateRangeReturnsNoResult(
            DateTime startDate, DateTime endDate, DateTime programmeStartDateTime)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateSponsorshipForDayPartsWithProgrammeRestrictionLevel(startDate, endDate);

            Programme programme = CreateProgramme(programmeStartDateTime);
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().BeEmpty();
        }

        [Theory(DisplayName = "Given one sponsorship record with two sponsorship items " +
         "When the programme start date time falls within the date range " +
         "Then two sponsorship item is returned")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-21T06:40:00")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-26T08:30:00")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-20T06:30:00")]
        public void OneSponsorshipRestrictions_ForGivenDateRangeReturnsTwoResult(
            DateTime startDate, DateTime endDate, DateTime programmeStartDateTime)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateSponsorshipForDayPartsWithProgrammeRestrictionLevel(
                    startDate,
                    endDate);

            Programme programme = CreateProgramme(programmeStartDateTime);
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle();
            _ = result[0].SponsoredItems.Should().ContainSingle();
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().HaveCount(2);
        }

        [Theory(DisplayName = "Given one sponsorship record " +
         "When the programme start date time falls within the date range " +
         "Then one sponsorship item is returned")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-21T06:40:00")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-26T08:30:00")]
        [InlineData("2020-04-20T00:00:00", "2020-04-26T00:00:00", "2020-04-20T06:30:00")]
        public void OneSponsorshipRestrictions_ForGivenDateRangeReturnsOneResult(
            DateTime startDate, DateTime endDate, DateTime programmeStartDateTime)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateSponsorshipForDayPartsWithProgrammeRestrictionLevel(
                    startDate,
                    endDate);
            listToFilter[0].SponsoredItems[1].SponsorshipItems.Last().StartDate = DateTime.MinValue;
            listToFilter[0].SponsoredItems[1].SponsorshipItems.Last().EndDate = DateTime.MinValue;

            Programme programme = CreateProgramme(programmeStartDateTime);
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle();
            _ = result[0].SponsoredItems.Should().ContainSingle();
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().ContainSingle();
        }

        private ImmutableList<Sponsorship> CreateSponsorshipForDayPartsWithProgrammeRestrictionLevel(
            DateTime startDate,
            DateTime endDate)
        {
            var sponsorshipRestrictions = _fixture.CreateMany<Sponsorship>().ToList();
            sponsorshipRestrictions[0].RestrictionLevel = SponsorshipRestrictionLevel.Programme;
            IEnumerable<SponsoredDayPart> dayPart = new List<SponsoredDayPart>()
            {
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] {
                        "Mon", "tue", "thursday", "Friday" },
                    StartTime = new TimeSpan(6, 30, 0),
                    EndTime = new TimeSpan(6, 59,59)
                },
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] {"SUNDAY", "sat" },
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(9, 59, 59)
                }
            };

            _fixture.Customize<SponsorshipItem>(
                obj => obj
                    .With(p => p.SalesAreas, new List<string> { "FindThisSalesArea" })
                    .With(p => p.ProgrammeName, "Pokémon")
                    .With(p => p.StartDate, startDate)
                    .With(p => p.EndDate, endDate)
                    .With(p => p.DayParts, dayPart)
                );

            var amendedList = sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems.ToList();
            amendedList.Add(_fixture.Create<SponsorshipItem>());
            amendedList.Add(_fixture.Create<SponsorshipItem>());
            sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems = amendedList;

            return sponsorshipRestrictions.ToImmutableList();
        }
    }
}
