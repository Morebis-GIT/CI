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
    [Collection(nameof(SmoothCollectionDefinition))]
    [Trait("Smooth", "Filter sponsorship restrictions")]
    public partial class SponsorshipRestrictionFilterServiceTests
    {
        private readonly Fixture _fixture;

        public SponsorshipRestrictionFilterServiceTests() => _fixture = new Fixture();

        [Fact(DisplayName = "Given null sponsorship records then null exception is thrown")]
        public void NullSponsorshipListThrows()
        {
            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
              {
                  // Arrange
                  var filter = new SponsorshipRestrictionFilterService(null);

                  // Act
                  var result = filter.Filter(null);
              });
        }

        [Fact(DisplayName = "Given a null programme then null exception is thrown")]
        public void NullProgrammeThrows()
        {
            // Assert
            _ = Assert.Throws<ArgumentNullException>(() =>
            {
                // Arrange
                var sponsorshipRestrictions = ImmutableList<Sponsorship>.Empty;
                var filter = new SponsorshipRestrictionFilterService(sponsorshipRestrictions);

                // Act
                var result = filter.Filter(null);
            });
        }

        [Fact(DisplayName = "Given no sponsorship records then no records are returned")]
        public void NoSponsorshipRecordsReturnsEmptyResults()
        {
            // Arrange
            var sponsorshipRestrictions = ImmutableList<Sponsorship>.Empty;
            var programme = new Programme();
            var filter = new SponsorshipRestrictionFilterService(sponsorshipRestrictions);

            // Act
            var result = filter.Filter(programme);

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Given one sponsorship records " +
           "When the given sales area and programme name does not match the Programme " +
           "Then nothing is returned")]
        public void SponsorshipRestrictionsNotForGivenSponsorshipReturnsZeroResults()
        {
            // Arrange
            var sponsorshipRestrictions = _fixture
                .CreateMany<Sponsorship>()
                .ToImmutableList();

            var programme = _fixture.Build<Programme>()
                .Create();
            var filter = new SponsorshipRestrictionFilterService(sponsorshipRestrictions);

            // Act
            var result = filter.Filter(programme);

            // Assert
            Assert.Empty(result);
        }

        [Fact(DisplayName = "Given one sponsorship record " +
       "When includes the given sales area and programme name " +
       "Then one sponsorship item is returned")]
        public void OneSponsorshipRestrictionsForGivenSponsorshipReturnsOneResult()
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter = CreateSponsorshipWithProgrammeRestrictionLevel();
            Programme programme = CreateProgramme();
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().HaveCount(2, becauseArgs: null);
        }

        [Fact(DisplayName = "Given one sponsorship record with two sponsorship items " +
          "When includes the given sales area and programme name in both " +
          "Then one sponsorship item is returned")]
        public void OneSponsorshipRestrictions_TwoSponsorshipItems_ForGivenSponsorshipReturnsOneResult()
        {
            // Arrange
            ImmutableList<Sponsorship> listToFilter = CreateSponsorshipWithProgrammeRestrictionLevel();

            var sponsorshipitemsList =
                listToFilter[0].SponsoredItems[1].SponsorshipItems.ToList();
            sponsorshipitemsList.Add(sponsorshipitemsList.Last());
            listToFilter[0].SponsoredItems[1].SponsorshipItems = sponsorshipitemsList;

            Programme programme = CreateProgramme();
            var filter = new SponsorshipRestrictionFilterService(listToFilter);

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().HaveCount(3, becauseArgs: null);
        }

        [Fact(DisplayName = "Given two sponsorship record with two sponsorship items " +
            "When includes the given sales area and programme name in both " +
            "Then two sponsorship item are returned")]
        public void TwoSponsorshipRestrictions_ForGivenSponsorshipReturnsTwoResult()
        {
            // Arrange
            var listToFilter = CreateSponsorshipWithProgrammeRestrictionLevel().ToList();
            listToFilter.AddRange(listToFilter);

            Programme programme = CreateProgramme();
            var filter = new SponsorshipRestrictionFilterService(listToFilter.ToImmutableList());

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().HaveCount(2, becauseArgs: null);
            _ = result[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[0].SponsoredItems[0].SponsorshipItems.Should().HaveCount(2, becauseArgs: null);
            _ = result[1].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = result[1].SponsoredItems[0].SponsorshipItems.Should().HaveCount(2, becauseArgs: null);
        }

        [Fact(DisplayName = "Given sponsorship record with two sponsorship items " +
            "When programme name does not match the programme " +
            "Then the number of sponsorship items should not change")]
        public void SponsorshipRestrictions_ForGivenSponsorshipReturnsNothingButDoesNotEffectSponsorshipItems()
        {
            // Arrange
            var sponsorship = new Sponsorship()
            {
                RestrictionLevel = SponsorshipRestrictionLevel.Programme,
                SponsoredItems = new List<SponsoredItem>()
                {
                    new SponsoredItem()
                    {
                        SponsorshipItems = new List<SponsorshipItem>()
                        {
                            new SponsorshipItem()
                            {
                                SalesAreas = new List<string> { "FindThisSalesArea" },
                                ProgrammeName = "Don't find this programme"
                            },
                            new SponsorshipItem()
                            {
                                SalesAreas = new List<string> { "Don't find this sales area" },
                                ProgrammeName = "Pokémon"
                            }
                        }
                    }
                }
            };

            var listToFilter = new List<Sponsorship>
            {
                sponsorship
            };

            var filter = new SponsorshipRestrictionFilterService(listToFilter.ToImmutableList());

            Programme programme = CreateProgramme();

            // Act
            var result = filter.Filter(programme);

            // Assert
            _ = result.Should().BeEmpty(becauseArgs: null);
            _ = listToFilter.Should().ContainSingle(becauseArgs: null);
            _ = listToFilter[0].SponsoredItems.Should().ContainSingle(becauseArgs: null);
            _ = listToFilter[0].SponsoredItems[0].SponsorshipItems.Should().HaveCount(2, becauseArgs: null);
        }

        private Programme CreateProgramme(DateTime? startDateTime = null) =>
            _fixture.Build<Programme>()
                .With(p => p.SalesArea, "FindThisSalesArea")
                .With(p => p.ProgrammeName, "Pokémon")
                .With(p => p.StartDateTime, startDateTime ?? DateTime.Now)
                .With(p => p.Duration, NodaTime.Duration.FromMinutes(30))
                .Create();

        private ImmutableList<Sponsorship> CreateSponsorshipWithProgrammeRestrictionLevel()
        {
            var sponsorshipRestrictions = _fixture.CreateMany<Sponsorship>().ToList();
            sponsorshipRestrictions[0].RestrictionLevel = SponsorshipRestrictionLevel.Programme;
            IEnumerable<SponsoredDayPart> dayPart = new List<SponsoredDayPart>()
            {
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] {
                        "Mon", "tue", "WED", "FRIDAY", "sunday" },
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(5, 59, 59)
                },
                new SponsoredDayPart()
                {
                    DaysOfWeek = new string[] {"THURSDAY", "sat" },
                    StartTime = new TimeSpan(6, 0, 0),
                    EndTime = new TimeSpan(5, 59, 59)
                }
            };

            _fixture.Customize<SponsorshipItem>(
                obj => obj
                    .With(p => p.SalesAreas, new List<string> { "FindThisSalesArea" })
                    .With(p => p.ProgrammeName, "Pokémon")
                    .With(p => p.StartDate, DateTime.Now.AddDays(-1).Date)
                    .With(p => p.EndDate, DateTime.Now.AddDays(1).Date)
                    .With(p => p.DayParts, dayPart)
                );

            var amendedList = sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems.ToList();
            // Add sponsorship item twice and set invalid data in individual tests
            amendedList.Add(_fixture.Create<SponsorshipItem>());
            amendedList.Add(_fixture.Create<SponsorshipItem>());

            sponsorshipRestrictions[0].SponsoredItems[1].SponsorshipItems = amendedList;

            return sponsorshipRestrictions.ToImmutableList();
        }
    }
}
