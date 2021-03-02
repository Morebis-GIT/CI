using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using Moq;
using NodaTime;
using Xunit;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.MockDataTests
{
    [Collection("Smooth")]
    [Trait("Smooth", "Sponsorship restriction service trigger recalculation of allowed limits")]
    public class SponsorshipRestrictionServiceTriggerRecalculationOfAllowedLimitsTests
    {
        private SponsorshipRestrictionService _service;
        private const SpotAction AddAction = SpotAction.AddSpot;

        private static readonly Break _theBreak = new Break()
        {
            ScheduledDate = new DateTime(2020, 5, 7, 7, 59, 59),
            Duration = Duration.FromSeconds(15)
        };

        private static readonly Guid _sponsoredSpotGuid = Guid.NewGuid();
        private static readonly Guid _advertiserCompetitorSpotGuid = Guid.NewGuid();
        private static readonly Guid _clashCompetitorSpotGuid = Guid.NewGuid();
        private static readonly Guid _advertiserAndClashCompetitorSpotGuid = Guid.NewGuid();
        private static readonly Guid _nonSponsorNonCompetitorSpotGuid = Guid.NewGuid();

        private const string SponsoredProduct = "sponsored product";
        private const string AdvertProduct = "advert product";
        private const string ClashProduct = "clash product";
        private const string AdvertAndClashProduct = "advert and clash product";

        private static readonly Mock<ISmoothSponsorshipTimelineManager> _mockTimeslines =
            new Mock<ISmoothSponsorshipTimelineManager>();

        [Fact(DisplayName = "Given a sponsored spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then event should be for a sponsored spot")]
        public void AddSponsoredSpot()
        {
            // Arrange
            Spot spot = GetSponsoredSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.AllCompetitors,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                _ = e.IsSponsor.Should().BeTrue(becauseArgs: null);
                _ = e.ProductExternalReference.Should().Be(SponsoredProduct, becauseArgs: null);
                _ = e.CompetitorType.Should().Be(SponsorshipCompetitorType.Neither, becauseArgs: null);
                _ = e.RestrictionType.type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a sponsored spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then event should be for a sponsored spot with duration unit")]
        public void AddDurationSponsoredSpot()
        {
            // Arrange
            Spot spot = GetSponsoredSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.AllCompetitors,
                    SponsorshipRestrictionType.SpotDuration,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                _ = e.IsSponsor.Should().BeTrue(becauseArgs: null);
                _ = e.ProductExternalReference.Should().Be(SponsoredProduct, becauseArgs: null);
                _ = e.CompetitorType.Should().Be(SponsorshipCompetitorType.Neither, becauseArgs: null);
                _ = e.RestrictionType.type.Should().Be(SponsorshipRestrictionType.SpotDuration, becauseArgs: null);
                _ = e.RestrictionType.duration.Should().Be(Duration.FromSeconds(15), becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a competitor spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then event should be for a competitor spot")]
        public void AddCompetitorSpot()
        {
            // Arrange
            Spot spot = GetAdvertiserCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.AllCompetitors,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                _ = e.IsSponsor.Should().BeFalse(becauseArgs: null);
                _ = e.ProductExternalReference.Should().Be(AdvertProduct, becauseArgs: null);
                _ = e.Applicability.Should().Be(SponsorshipApplicability.AllCompetitors, becauseArgs: null);
                _ = e.CompetitorType.Should().Be(SponsorshipCompetitorType.Advertiser, becauseArgs: null);
                _ = e.RestrictionType.type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a clash competitor spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then event should be for a clash competitor spot")]
        public void AddClashCompetitorSpot()
        {
            // Arrange
            Spot spot = GetClashCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                _ = e.IsSponsor.Should().BeFalse(becauseArgs: null);
                _ = e.ProductExternalReference.Should().Be(ClashProduct, becauseArgs: null);
                _ = e.Applicability.Should().Be(SponsorshipApplicability.EachCompetitor, becauseArgs: null);
                _ = e.CompetitorType.Should().Be(SponsorshipCompetitorType.Clash, becauseArgs: null);
                _ = e.RestrictionType.type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a advertiser competitor spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then event should be for a advertiser competitor spot")]
        public void AddAdvertiserCompetitorSpot()
        {
            // Arrange
            Spot spot = GetAdvertiserCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                _ = e.IsSponsor.Should().BeFalse(becauseArgs: null);
                _ = e.ProductExternalReference.Should().Be(AdvertProduct, becauseArgs: null);
                _ = e.Applicability.Should().Be(SponsorshipApplicability.EachCompetitor, becauseArgs: null);
                _ = e.CompetitorType.Should().Be(SponsorshipCompetitorType.Advertiser, becauseArgs: null);
                _ = e.RestrictionType.type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a advertiser and clash competitor spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then event should be for a advertiser and clash competitor spot")]
        public void AddAdvertiserAndClashCompetitorSpot()
        {
            // Arrange
            Spot spot = GetAdvertiserClashCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                _ = e.IsSponsor.Should().BeFalse(becauseArgs: null);
                _ = e.ProductExternalReference.Should().Be(AdvertAndClashProduct, becauseArgs: null);
                _ = e.Applicability.Should().Be(SponsorshipApplicability.EachCompetitor, becauseArgs: null);
                _ = e.CompetitorType.Should().Be(SponsorshipCompetitorType.Both, becauseArgs: null);
                _ = e.RestrictionType.type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a non advertiser and non clash spot is added " +
            "When triggering the recalculation of restriction allowance " +
            "Then no event should be raised")]
        public void AddNonSponsorNonComepetitorSpot()
        {
            // Arrange
            Spot spot = GetNonSponsorNonCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseAddedSpotToBreakEvent += RaiseAddedSpotToBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(AddAction, spot, _theBreak);

            // Assert
            void RaiseAddedSpotToBreakEvent(
                object sender,
                AddedSpotToBreakEventArgs e)
            {
                Assert.Null(sender);
                Assert.Null(e);
            }
        }

        [Fact(DisplayName = "Given a remove sponsored spot action is triggered " +
            "When triggering the recalculation of restriction allowance " +
            "Then remove event should be raised")]
        public void RemoveDurationSponsoredSpot()
        {
            // Arrange
            const SpotAction removeAction = SpotAction.RemoveSpot;
            Spot spot = GetSponsoredSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotDuration,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseRemovedSpotFromBreakEvent += RaiseRemovedSpotFromBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(removeAction, spot, _theBreak);

            // Assert
            void RaiseRemovedSpotFromBreakEvent(object sender, RemovedSpotFromBreakEventArgs e)
            {
                _ = e.ProductExternalReference.Should().Be(SponsoredProduct, becauseArgs: null);
                _ = e.IsSponsor.Should().BeTrue(becauseArgs: null);
                _ = e.RestrictionType.Type.Should().Be(SponsorshipRestrictionType.SpotDuration, becauseArgs: null);
                _ = e.RestrictionType.Duration.Should().Be(Duration.FromSeconds(15), becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a remove sponsored spot action has been triggered " +
            "When triggering the recalculation of restriction allowance " +
            "Then remove event should be raised")]
        public void RemoveCountSponsoredSpot()
        {
            // Arrange
            const SpotAction removeAction = SpotAction.RemoveSpot;
            Spot spot = GetSponsoredSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseRemovedSpotFromBreakEvent += RaiseRemovedSpotFromBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(removeAction, spot, _theBreak);

            // Assert
            void RaiseRemovedSpotFromBreakEvent(object sender, RemovedSpotFromBreakEventArgs e)
            {
                _ = e.ProductExternalReference.Should().Be(SponsoredProduct, becauseArgs: null);
                _ = e.IsSponsor.Should().BeTrue(becauseArgs: null);
                _ = e.RestrictionType.Type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.Duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a remove competitor spot action is triggered " +
            "When triggering the recalculation of restriction allowance " +
            "Then remove event should be raised")]
        public void RemoveDurationCompetitorSpot()
        {
            // Arrange
            const SpotAction removeAction = SpotAction.RemoveSpot;
            Spot spot = GetAdvertiserClashCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotDuration,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseRemovedSpotFromBreakEvent += RaiseRemovedSpotFromBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(removeAction, spot, _theBreak);

            // Assert
            void RaiseRemovedSpotFromBreakEvent(object sender, RemovedSpotFromBreakEventArgs e)
            {
                _ = e.ProductExternalReference.Should().Be(AdvertAndClashProduct, becauseArgs: null);
                _ = e.IsSponsor.Should().BeFalse(becauseArgs: null);
                _ = e.RestrictionType.Type.Should().Be(SponsorshipRestrictionType.SpotDuration, becauseArgs: null);
                _ = e.RestrictionType.Duration.Should().Be(Duration.FromSeconds(15), becauseArgs: null);
            }
        }

        [Fact(DisplayName = "Given a remove competitor spot action has been triggered " +
            "When triggering the recalculation of restriction allowance " +
            "Then remove event should be raised")]
        public void RemoveCountCompetitorSpot()
        {
            // Arrange
            const SpotAction removeAction = SpotAction.RemoveSpot;
            Spot spot = GetAdvertiserClashCompetitorSpot();

            _service = new SponsorshipRestrictionService(
                GetSponsorshipRestrictionFilterResults(
                    SponsorshipApplicability.EachCompetitor,
                    SponsorshipRestrictionType.SpotCount,
                    5,
                    SponsorshipCalculationType.Percentage),
                _mockTimeslines.Object,
                GetSpotInfo(),
                null);

            // Act
            _service.RaiseRemovedSpotFromBreakEvent += RaiseRemovedSpotFromBreakEvent;
            _service.TriggerRecalculationOfAllowedRestrictionLimits(removeAction, spot, _theBreak);

            // Assert
            void RaiseRemovedSpotFromBreakEvent(object sender, RemovedSpotFromBreakEventArgs e)
            {
                _ = e.ProductExternalReference.Should().Be(AdvertAndClashProduct, becauseArgs: null);
                _ = e.IsSponsor.Should().BeFalse(becauseArgs: null);
                _ = e.RestrictionType.Type.Should().Be(SponsorshipRestrictionType.SpotCount, becauseArgs: null);
                _ = e.RestrictionType.Duration.Should().Be(Duration.Zero, becauseArgs: null);
            }
        }

        private static Spot GetSponsoredSpot() =>
            new Spot()
            {
                Uid = _sponsoredSpotGuid,
                Product = SponsoredProduct,
                SpotLength = Duration.FromSeconds(15)
            };

        private static Spot GetClashCompetitorSpot() =>
            new Spot()
            {
                Uid = _clashCompetitorSpotGuid,
                Product = ClashProduct
            };

        private static Spot GetAdvertiserCompetitorSpot() =>
            new Spot()
            {
                Uid = _advertiserCompetitorSpotGuid,
                Product = AdvertProduct
            };

        private static Spot GetAdvertiserClashCompetitorSpot() =>
            new Spot()
            {
                Uid = _advertiserAndClashCompetitorSpotGuid,
                Product = AdvertAndClashProduct,
                SpotLength = Duration.FromSeconds(15)
            };

        private static Spot GetNonSponsorNonCompetitorSpot() =>
            new Spot()
            {
                Uid = _nonSponsorNonCompetitorSpotGuid
            };

        private static Dictionary<Guid, SpotInfo> GetSpotInfo()
        {
            return new List<SpotInfo>()
            {
                new SpotInfo()
                {
                    ProductClashCode = "1",
                    ProductAdvertiserIdentifier = "sponsor",
                    Uid = _sponsoredSpotGuid
                },new SpotInfo()
                {
                    ProductClashCode = "2",
                    ProductAdvertiserIdentifier = "advertiser",
                    Uid = _advertiserCompetitorSpotGuid
                },new SpotInfo()
                {
                    ProductClashCode = "3",
                    ProductAdvertiserIdentifier = "clash",
                    Uid = _clashCompetitorSpotGuid
                },new SpotInfo()
                {
                    ProductClashCode = "3",
                    ProductAdvertiserIdentifier = "advertiser",
                    Uid = _advertiserAndClashCompetitorSpotGuid
                },new SpotInfo()
                {
                    ProductClashCode = "5",
                    ProductAdvertiserIdentifier = "normal",
                    Uid = _nonSponsorNonCompetitorSpotGuid
                }
             }
            .ToDictionary(s => s.Uid);
        }

        private static List<SponsorshipRestrictionFilterResults>
            GetSponsorshipRestrictionFilterResults(
            SponsorshipApplicability sponsorshipApplicability,
            SponsorshipRestrictionType sponsorshipRestrictionType,
            int restrictionValue,
            SponsorshipCalculationType sponsorshipCalculationType)
        {
            return new List<SponsorshipRestrictionFilterResults>()
            {
                new SponsorshipRestrictionFilterResults()
                {
                    SponsoredItems = new List<SponsoredItem>()
                    {
                        new SponsoredItem()
                        {
                            Applicability = sponsorshipApplicability,
                            AdvertiserExclusivities = new List<AdvertiserExclusivity>()
                            {
                                new AdvertiserExclusivity()
                                {
                                    AdvertiserIdentifier = "advertiser",
                                    RestrictionType = sponsorshipRestrictionType,
                                    RestrictionValue = restrictionValue
                                }
                            },
                            ClashExclusivities = new List<ClashExclusivity>()
                            {
                                new ClashExclusivity()
                                {
                                    ClashExternalRef = "3",
                                    RestrictionType = sponsorshipRestrictionType,
                                    RestrictionValue = restrictionValue
                                }
                            },
                            Products = new List<string>()
                            {
                                SponsoredProduct
                            },
                            RestrictionType = sponsorshipRestrictionType,
                            RestrictionValue = restrictionValue,
                            CalculationType = sponsorshipCalculationType,
                            SponsorshipItems = new List<SponsorshipItem>()
                            {
                                new SponsorshipItem()
                                {
                                    StartDate = new DateTime(2020,5,7),
                                    EndDate = new DateTime(2020,5,7),
                                    DayParts = new List<SponsoredDayPart>()
                                    {
                                        new SponsoredDayPart()
                                        {
                                            StartTime = new TimeSpan(8,0,0),
                                            EndTime = new TimeSpan(10,0,0),
                                            DaysOfWeek = new string[] { "Thursday" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    .ToImmutableList()
                }
            };
        }
    }
}
