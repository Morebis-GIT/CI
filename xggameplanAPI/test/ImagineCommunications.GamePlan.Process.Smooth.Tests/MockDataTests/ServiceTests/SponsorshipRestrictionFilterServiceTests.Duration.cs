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
            "When the sponsorship falls completely out of the programme " +
            "Then no sponsorship item is returned")]
        [InlineData("15:00:00", "17:00:00")]
        [InlineData("20:00:00", "21:00:00")]
        public void OneSponsorshipRestrictions_ForGivenDurationReturnsNoResult(
              string dayPartStartTime, string dayPartEndTime)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateSponsorshipForDurationsWithProgrammeRestrictionLevel(
                    TimeSpan.Parse(dayPartStartTime), TimeSpan.Parse(dayPartEndTime));

            Programme programme = CreateProgrammeForDuration(new DateTime(2020, 4, 21, 17, 0, 0));
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().BeEmpty(becauseArgs: null);
        }

        [Theory(DisplayName = "Given one sponsorship record " +
         "When the sponsorship falls fully or partly within the programme " +
         "Then one sponsorship item is returned")]
        [InlineData("17:30:00", "20:30:00")]
        [InlineData("18:00:00", "19:00:00")]
        [InlineData("17:00:00", "20:00:00")]
        [InlineData("16:00:00", "21:00:00")]
        [InlineData("16:00:00", "17:30:00")]
        public void OneSponsorshipRestrictions_ForGivenDurationReturnsTwoResult(
              string dayPartStartTime, string dayPartEndTime)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateSponsorshipForDurationsWithProgrammeRestrictionLevel(
                    TimeSpan.Parse(dayPartStartTime),
                    TimeSpan.Parse(dayPartEndTime));

            Programme programme = CreateProgrammeForDuration(new DateTime(2020, 4, 21, 17, 0, 0));
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().HaveCount(2, becauseArgs: null);
        }

        [Theory(DisplayName = "Given one sponsorship record " +
            "When the sponsorship spans or after midnight and the programme is " +
            "before, spans or after midnight " +
            "Then one sponsorship item is returned")]
        [InlineData("23:30:00", "02:00:00", "2020-04-22T00:05:00")]
        [InlineData("01:00:00", "02:00:00", "2020-04-22T00:05:00")]
        [InlineData("23:30:00", "02:00:00", "2020-04-21T22:30:00")]
        [InlineData("01:00:00", "02:00:00", "2020-04-21T22:30:00")]
        [InlineData("21:00:00", "23:50:00", "2020-04-21T22:30:00")]
        [InlineData("23:30:00", "02:00:00", "2020-04-21T20:50:00")]
        [InlineData("21:00:00", "23:50:00", "2020-04-21T20:50:00")]
        public void MidnightAndBeforeNextBroadcastDay(
              string dayPartStartTime,
              string dayPartEndTime,
              DateTime programmeStartDate)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateAfterMidnightSponsorshipForDurationsWithProgrammeRestrictionLevel(
                    TimeSpan.Parse(dayPartStartTime),
                    TimeSpan.Parse(dayPartEndTime));

            var programme = CreateProgrammeForDuration(programmeStartDate);

            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().ContainSingle(becauseArgs: null);
        }

        [Theory(DisplayName = "Given two sponsorship item record " +
         "When the one sponsorship falls fully or partly within the programme and one does not " +
         "Then one sponsorship item is returned")]
        [InlineData("17:30:00", "20:30:00")]
        [InlineData("18:00:00", "19:00:00")]
        [InlineData("17:00:00", "20:00:00")]
        [InlineData("16:00:00", "21:00:00")]
        [InlineData("16:00:00", "17:30:00")]
        public void TwoSponsorshipRestrictionsItems_ForGivenDurationReturnsOneResult(
              string dayPartStartTime, string dayPartEndTime)
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter =
                CreateSponsorshipForDurationsWithProgrammeRestrictionLevel(
                    TimeSpan.Parse(dayPartStartTime),
                    TimeSpan.Parse(dayPartEndTime));

            listToFilter[0].SponsoredItems[1].SponsorshipItems.Last().DayParts =
                 new List<SponsoredDayPart>()
            {
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] {
                       "Sun", "Mon", "Tue", "Wed", "Thu", },
                    StartTime = new TimeSpan(),
                    EndTime = new TimeSpan()
                },
            };
            Programme programme = CreateProgrammeForDuration(new DateTime(2020, 4, 21, 17, 0, 0));
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().ContainSingle(becauseArgs: null);
        }

        private Programme CreateProgrammeForDuration(DateTime startDateTime) =>
         _fixture.Build<Programme>()
         .With(p => p.SalesArea, "FindThisSalesArea")
         .With(p => p.ProgrammeName, "Transformers: Dark of the Moon")
         .With(p => p.StartDateTime, startDateTime)
         .With(p => p.Duration, NodaTime.Duration.FromHours(3))
         .Create();

        private ImmutableList<Sponsorship>
            CreateSponsorshipForDurationsWithProgrammeRestrictionLevel(
            TimeSpan dayPartStartTime,
            TimeSpan dayPartEndTime)
        {
            var sponsorshipRestrictions = _fixture.CreateMany<Sponsorship>().ToList();
            sponsorshipRestrictions[0].RestrictionLevel = SponsorshipRestrictionLevel.Programme;
            IEnumerable<SponsoredDayPart> dayPart = new List<SponsoredDayPart>()
            {
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] {"Tue" },
                    StartTime = dayPartStartTime,
                    EndTime = dayPartEndTime
                },
            };

            _fixture.Customize<SponsorshipItem>(
                obj => obj
                    .With(p => p.SalesAreas, new List<string> { "FindThisSalesArea" })
                    .With(p => p.ProgrammeName, "Transformers: Dark of the Moon")
                    .With(p => p.StartDate, new DateTime(2020, 4, 21))
                    .With(p => p.EndDate, new DateTime(2020, 4, 21))
                    .With(p => p.DayParts, dayPart)
                );

            var amendedList = sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems.ToList();
            amendedList.Add(_fixture.Create<SponsorshipItem>());
            amendedList.Add(_fixture.Create<SponsorshipItem>());
            sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems = amendedList;

            return sponsorshipRestrictions.ToImmutableList();
        }

        private ImmutableList<Sponsorship>
            CreateAfterMidnightSponsorshipForDurationsWithProgrammeRestrictionLevel(
            TimeSpan dayPartStartTime,
            TimeSpan dayPartEndTime)
        {
            var sponsorshipRestrictions = new Sponsorship
            {
                RestrictionLevel = SponsorshipRestrictionLevel.TimeBand
            };

            IEnumerable<SponsoredDayPart> dayPart = new List<SponsoredDayPart>()
            {
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] { "Tue" },
                    StartTime = dayPartStartTime,
                    EndTime = dayPartEndTime
                },
            };

            _fixture.Customize<SponsorshipItem>(
                obj => obj
                    .With(p => p.SalesAreas, new List<string> { "FindThisSalesArea" })
                    .With(p => p.ProgrammeName, "Transformers: Dark of the Moon")
                    .With(p => p.StartDate, new DateTime(2020, 4, 21))
                    .With(p => p.EndDate, new DateTime(2020, 4, 21))
                    .With(p => p.DayParts, dayPart)
                );

            sponsorshipRestrictions.SponsoredItems = new List<SponsoredItem>()
            {
                new SponsoredItem()
                {
                    SponsorshipItems = new List<SponsorshipItem>()
                    {
                        _fixture.Create<SponsorshipItem>()
                    }
                }
            };

            return new List<Sponsorship>() { sponsorshipRestrictions }.ToImmutableList();
        }
    }
}
