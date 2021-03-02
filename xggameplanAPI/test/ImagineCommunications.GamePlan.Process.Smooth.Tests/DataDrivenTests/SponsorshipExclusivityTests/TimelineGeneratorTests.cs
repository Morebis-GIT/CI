using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers.CollectionFixtures;
using Xunit;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "XG10/11 Sponsorship")]
    public class TimelineGeneratorTests : SponsorshipRestrictionsTests
    {
        private readonly Fixture _fixture;
        private readonly DateTimeRange _dateTimeRange;
        private readonly string _salesArea;

        public TimelineGeneratorTests(SmoothFixture smoothFixture,
            ITestOutputHelper output)
            : base(smoothFixture, output, false)
        {
            _dateTimeRange = (new DateTime(2020, 04, 20, 3, 0, 0),
                new DateTime(2020, 04, 27, 2, 59, 59));
            _fixture = new Fixture();
            _salesArea = "GTV94";
        }

        [Fact(DisplayName = "Given invalid date time range " +
            "When extracting timelines from sponsorships " +
            "Then throw exception")]
        public void NullDateTimeRangeThrows()
        {
            // Arrange
            var extract = new TimelineGenerator(new DateTimeRange(), null, _salesArea);

            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var result = extract.ExtractTimelines();
            });
        }

        [Fact(DisplayName = "Given sponsorship is null " +
           "When extracting timelines from sponsorships " +
           "Then throw exception")]
        public void NullSponsorshipThrows()
        {
            // Arrange
            var extract = new TimelineGenerator(_dateTimeRange, null, _salesArea);

            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var result = extract.ExtractTimelines();
            });
        }

        [Fact(DisplayName = "Given sponsorship is empty " +
        "When extracting timelines from sponsorships " +
        "Then empty timeline list is returned")]
        public void EmptySponsorshipReturnsEmptyTimeline()
        {
            // Arrange
            var extract = new TimelineGenerator(_dateTimeRange, ImmutableList.Create<Sponsorship>(), _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Given sponsorship list only contains restriction " +
            "level programme " +
            "When extracting timelines from sponsorships " +
            "Then empty timeline list is returned")]
        public void ProgrammeSponsorshipReturnsEmptyTimeline()
        {
            // Arrange
            var sponsorships = _fixture.CreateMany<Sponsorship>().ToList();
            foreach (var s in sponsorships)
            {
                s.RestrictionLevel = SponsorshipRestrictionLevel.Programme;
            }

            var extract = new TimelineGenerator(
                _dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Given sponsorship list only contains restriction " +
            "level programme and time bands " +
            "When extracting timelines from sponsorships " +
            "Then only time band timeline list should be returned")]
        public void TimebandAndProgrammeSponsorshipReturnsTimeline()
        {
            // Arrange
            IEnumerable<Sponsorship> sponsorships = LoadSponsorships("BigOne");
            var extract = new TimelineGenerator(
                _dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            _ = result.Should().HaveCount(18, becauseArgs: null);
        }

        [Fact(DisplayName = "Given sponsorship list only contains restriction " +
            "level programme and time bands and no day parts or dates match " +
            "When extracting timelines from sponsorships " +
            "Then empty timeline list should be returned")]
        public void TimebandAndProgrammeSponsorshipReturnsEmptyTimeline()
        {
            // Arrange
            IEnumerable<Sponsorship> sponsorships = LoadSponsorships("Empty");

            var extract = new TimelineGenerator(_dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Given sponsorship list only contains restriction " +
            "time bands " +
            "When extracting timelines from sponsorships " +
            "Then correct times should be returned")]
        public void TimesSponsorshipReturnsTimelineList()
        {
            // Arrange
            IEnumerable<Sponsorship> sponsorships = LoadSponsorships("Times");

            var extract = new TimelineGenerator(
                _dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            _ = result.Should().HaveCount(7, becauseArgs: null);
            result.ForEach(t =>
            {
                _ = t.DateTimeRange.Start.TimeOfDay.Should().Equals(new TimeSpan(8, 0, 0));
                _ = t.DateTimeRange.End.TimeOfDay.Should().Equals(new TimeSpan(18, 59, 59));
            });
        }

        [Fact(DisplayName = "Given sponsorship list only contains restriction " +
            "time bands " +
            "When extracting timelines from sponsorships " +
            "Then correct dates should be returned")]
        public void DatesSponsorshipReturnsTimelineList()
        {
            // Arrange
            const string folderName = "Dates";
            IEnumerable<Sponsorship> sponsorships = LoadSponsorships(folderName);

            var extract = new TimelineGenerator(_dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            _ = result.Should().HaveCount(7, becauseArgs: null);
            result.ForEach(t =>
            {
                _ = t.DateTimeRange.Start.Date.Should().Equals(new DateTime(2020, 04, 22));
                _ = t.DateTimeRange.End.Date.Should().Equals(new DateTime(2020, 04, 22));
            });
        }

        [Fact(DisplayName = "Given sponsorship goes over midnight " +
            "When extracting timelines from sponsorships " +
            "Then correct dates and times should be returned")]
        public void MidnightSponsorshipReturnsTimelineList()
        {
            // Arrange
            const string folderName = "Midnight";
            IEnumerable<Sponsorship> sponsorships = LoadSponsorships(folderName);

            var extract = new TimelineGenerator(_dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].DateTimeRange.Start.Date.Should().Be(new DateTime(2020, 04, 22));
            _ = result[0].DateTimeRange.Start.TimeOfDay.Should().Be(new TimeSpan(23, 30, 0));
            _ = result[0].DateTimeRange.End.Date.Should().Be(new DateTime(2020, 04, 23));
            _ = result[0].DateTimeRange.End.TimeOfDay.Should().Be(new TimeSpan(0, 29, 59));
        }

        [Fact(DisplayName = "Given sponsorship is after midnight " +
            "When extracting timelines from sponsorships " +
            "Then correct dates and times should be returned")]
        public void AfterMidnightSponsorshipReturnsTimelineList()
        {
            // Arrange
            const string folderName = "AfterMidnight";
            IEnumerable<Sponsorship> sponsorships = LoadSponsorships(folderName);

            var extract = new TimelineGenerator(_dateTimeRange,
                sponsorships.ToImmutableList(),
                _salesArea);

            // Act
            var result = extract.ExtractTimelines();

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].DateTimeRange.Start.Date.Should().Be(new DateTime(2020, 04, 23));
            _ = result[0].DateTimeRange.Start.TimeOfDay.Should().Be(new TimeSpan(1, 0, 0));
            _ = result[0].DateTimeRange.End.Date.Should().Be(new DateTime(2020, 04, 23));
            _ = result[0].DateTimeRange.End.TimeOfDay.Should().Be(new TimeSpan(1, 59, 59));
        }

        private IEnumerable<Sponsorship> LoadSponsorships(string folderName)
        {
            DataLoader.LoadTestSpecificRepositories(
                RepositoryFactory,
                Path.Combine("SponsorshipExclusivity",
                "TimelineGenerator",
                folderName));

            IEnumerable<Sponsorship> sponsorships = null;

            using (var scope = RepositoryFactory.BeginRepositoryScope())
            {
                var repo = scope.CreateRepository<ISponsorshipRepository>();
                sponsorships = repo.GetAll();
            }

            return sponsorships.Where(s => s.ExternalReferenceId.Contains(folderName));
        }
    }
}
