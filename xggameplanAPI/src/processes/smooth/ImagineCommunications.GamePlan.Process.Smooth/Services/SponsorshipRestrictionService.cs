using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos.EventArguments;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Apply sponsorship restriction rules to spots. If any restrictions apply,
    /// return an advisory failure code.
    /// </summary>
    public class SponsorshipRestrictionService
    {
        private readonly IReadOnlyList<SponsorshipRestrictionFilterResults> _sponsorshipRestrictions;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;
        private Action<string, Exception> RaiseException { get; }
        public ISmoothSponsorshipTimelineManager TimelineManager { get; set; }

        public SponsorshipRestrictionService(
            IReadOnlyList<SponsorshipRestrictionFilterResults> programmeSponsorshipRestrictions,
            ISmoothSponsorshipTimelineManager timelineManager,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            Action<string, Exception> raiseException)
        {
            _sponsorshipRestrictions = programmeSponsorshipRestrictions;
            _spotInfos = spotInfos;
            RaiseException = raiseException;
            TimelineManager = timelineManager;
        }

        /// <summary>
        /// Declares an event that is raised when a spot is placed in a break.
        /// </summary>
        public event EventHandler<AddedSpotToBreakEventArgs> RaiseAddedSpotToBreakEvent
            = delegate { };

        /// <summary>
        /// Declares an event that is raised when a spot is removed from a break.
        /// </summary>
        public event EventHandler<RemovedSpotFromBreakEventArgs> RaiseRemovedSpotFromBreakEvent
            = delegate { };

        protected virtual void OnRaiseAddedSpotToBreakEvent(
            AddedSpotToBreakEventArgs e
            ) => RaiseAddedSpotToBreakEvent?.Invoke(this, e);

        protected virtual void OnRaiseRemovedSpotFromBreakEvent(
            RemovedSpotFromBreakEventArgs e
            ) => RaiseRemovedSpotFromBreakEvent?.Invoke(this, e);

        /// <summary>
        /// Empty list to use when there's nothing to return.
        /// </summary>
        private static
        IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)> NoSponsorshipRestrictionFailures
        { get; }
            = new List<(Guid spotUid, SmoothFailureMessages failureMessage)>(0);

        /// <summary>
        /// Evaluate sponsorship restrictions for the given spot in relation to
        /// other spots already placed in the break.
        /// </summary>
        /// <param name="spotToCheckForRestrictions">
        /// A candidate spot for the break, assuming no restrictions block its placement.
        /// </param>
        /// <param name="externalBreakRef"></param>
        /// <param name="breakScheduledDate">
        /// When the break is scheduled for transmission as UTC.
        /// </param>
        /// <param name="breakDuration"></param>
        /// <param name="spotsAlreadyInTheBreak">
        /// Spots already placed in the break.
        /// </param>
        public IReadOnlyCollection<(Guid spotUid, SmoothFailureMessages failureMessage)>
        CheckSponsorshipRestrictions(
            Spot spotToCheckForRestrictions,
            string breakExternalReference,
            DateTime breakScheduledDate,
            Duration breakDuration,
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak)
        {
            if (String.IsNullOrWhiteSpace(breakExternalReference))
            {
                throw new ArgumentException(
                    "Sponsorship restrictions cannot be checked without a break external reference.",
                    nameof(breakExternalReference)
                    );
            }

            if (_sponsorshipRestrictions.Count == 0)
            {
                return NoSponsorshipRestrictionFailures;
            }

            IReadOnlyCollection<SponsoredItem> sponsoredItems = _sponsorshipRestrictions
                .SelectMany(ss => ss.SponsoredItems)
                .ToList();

            if (sponsoredItems.Count == 0)
            {
                return NoSponsorshipRestrictionFailures;
            }

            Guid spotUid = spotToCheckForRestrictions.Uid;
            string product = spotToCheckForRestrictions.Product;
            var breakStartTime = breakScheduledDate.TimeOfDay;
            var breakEndTime = breakStartTime.Add(breakDuration.ToTimeSpan());

            var result = new List<(Guid spotUid, SmoothFailureMessages failureMessage)>();

            foreach (SponsoredItem item in sponsoredItems)
            {
                IReadOnlyCollection<SponsorshipItem> restrictionsForThisBreak
                    = GetRestrictionsForThisBreak(
                        item,
                        breakStartTime,
                        breakEndTime);

                if (restrictionsForThisBreak.Count == 0)
                {
                    continue;
                }

                IReadOnlyCollection<string> sponsoredProductsExternalRefs = item.Products.ToList() ?? new List<string>(0);
                IReadOnlyCollection<ClashExclusivity> clashExclusivities = item.ClashExclusivities ?? new List<ClashExclusivity>(0);
                IReadOnlyCollection<AdvertiserExclusivity> advertiserExclusivities = item.AdvertiserExclusivities ?? new List<AdvertiserExclusivity>(0);

                bool isThisSpotACompetitorByClashCode = IsThisACompetitorByClashCode(spotUid, clashExclusivities);
                bool isThisSpotACompetitorByAdvertiser = IsThisACompetitorByAdvertiserCode(spotUid, advertiserExclusivities);

                switch (item.CalculationType)
                {
                    case SponsorshipCalculationType.None:
                        AddFailureIfSpotIsACompetitorAndBreakAlreadyContainsASponsoredSpot(
                            spotUid,
                            spotsAlreadyInTheBreak,
                            result,
                            sponsoredProductsExternalRefs,
                            isThisSpotACompetitorByClashCode,
                            isThisSpotACompetitorByAdvertiser);

                        AddFailureIfSpotIsASponsorAndBreakAlreadyContainsACompetitorSpot(
                            spotUid,
                            product,
                            spotsAlreadyInTheBreak,
                            result,
                            sponsoredProductsExternalRefs,
                            clashExclusivities,
                            advertiserExclusivities);

                        break;

                    case SponsorshipCalculationType.Exclusive:
                        if (!IsSpotProductASponsor(product, sponsoredProductsExternalRefs))
                        {
                            AddFailureIfSpotIsACompetitor(
                                spotUid,
                                result,
                                isThisSpotACompetitorByClashCode,
                                isThisSpotACompetitorByAdvertiser);
                        }

                        break;

                    case SponsorshipCalculationType.Flat:
                    case SponsorshipCalculationType.Percentage:
                        {
                            bool breakAlreadyContainASponsoredSpot = DoesBreakAlreadyContainASponsoredSpot(
                                spotsAlreadyInTheBreak,
                                sponsoredProductsExternalRefs);

                            bool spotIsASponsor = IsSpotProductASponsor(product, sponsoredProductsExternalRefs);

                            if (spotIsASponsor)
                            {
                                AddFailureIfSpotIsASponsorAndBreakAlreadyContainsACompetitorSpot(
                                    spotUid,
                                    product,
                                    spotsAlreadyInTheBreak,
                                    result,
                                    sponsoredProductsExternalRefs,
                                    clashExclusivities,
                                    advertiserExclusivities);
                            }
                            else if (breakAlreadyContainASponsoredSpot)
                            {
                                AddFailureIfSpotIsACompetitor(
                                    spotUid,
                                    result,
                                    isThisSpotACompetitorByClashCode,
                                    isThisSpotACompetitorByAdvertiser);
                            }
                            else if (isThisSpotACompetitorByAdvertiser ||
                                isThisSpotACompetitorByClashCode)
                            {
                                /*
                                 * For a given spot, break and set of restrictions,
                                 * could this spot be added to the break?
                                 *
                                 * If not, add a Smooth Failure of the correct type.
                                 *
                                 */
                                try
                                {
                                    SmoothSponsorshipRestrictionLimits restrictionLimits =
                                        TimelineManager
                                        .FindTimelineForCompetitor(
                                        breakExternalReference,
                                        _spotInfos[spotUid].ProductAdvertiserIdentifier,
                                        _spotInfos[spotUid].ProductClashCode)
                                       .RestrictionLimits;

                                    var (isSpotAllowed, failureMessage) = SpotInspectorService
                                        .InspectSpot(
                                        item,
                                        restrictionLimits,
                                        spotToCheckForRestrictions.Product,
                                        spotToCheckForRestrictions.SpotLength,
                                        (isThisSpotACompetitorByAdvertiser,
                                        _spotInfos[spotUid].ProductAdvertiserIdentifier),
                                        (isThisSpotACompetitorByClashCode,
                                        _spotInfos[spotUid].ProductClashCode));

                                    if (!isSpotAllowed)
                                    {
                                        foreach (var message in failureMessage)
                                        {
                                            result.Add(
                                                (spotUid,
                                                message));
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    RaiseException("Unable to find a sponsorship timeline" +
                                        $" when checking calculation type {item.CalculationType}, " +
                                        $"applicability {item.Applicability}, restriction " +
                                        $"type {item.RestrictionType} " +
                                        $"for spot {spotToCheckForRestrictions.ExternalSpotRef} ",
                                        ex);
                                }
                            }

                            break;
                        }
                }
            }

            return result;
        }

        private IReadOnlyCollection<SponsorshipItem> GetRestrictionsForThisBreak(
            SponsoredItem item,
            TimeSpan breakStartTime,
            TimeSpan breakEndTime)
        {
            var restrictionsForThisBreak = new List<SponsorshipItem>();

            foreach (SponsorshipItem si in item.SponsorshipItems)
            {
                restrictionsForThisBreak.AddRange(
                    si.DayParts
                        .Where(DaypartMatchesThisBreak)
                        .Select(_ => si)
                    );
            }

            return restrictionsForThisBreak;

            bool DaypartMatchesThisBreak(SponsoredDayPart dp) =>
                TimeHelper.TimeRangeOverlaps(
                    dp.StartTime,
                    dp.EndTime,
                    breakStartTime,
                    breakEndTime);
        }

        private void AddFailureIfSpotIsACompetitor(
            Guid spotUid,
            ICollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result,
            bool isThisSpotACompetitorByClashCode,
            bool isThisSpotACompetitorByAdvertiser)
        {
            if (isThisSpotACompetitorByClashCode)
            {
                AddCompetitorByClashRestrictionFailure(spotUid, result);
            }

            if (isThisSpotACompetitorByAdvertiser)
            {
                AddCompetitorByAdvertiserRestrictionFailure(spotUid, result);
            }
        }

        /// <summary>
        /// If a break contains a competitor spot do not place any sponsored
        /// spots in the break.
        /// </summary>
        private void AddFailureIfSpotIsASponsorAndBreakAlreadyContainsACompetitorSpot(
            Guid spotUid,
            string product,
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            ICollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result,
            IReadOnlyCollection<string> sponsoredProductsExternalRefs,
            IReadOnlyCollection<ClashExclusivity> clashExclusivities,
            IReadOnlyCollection<AdvertiserExclusivity> advertiserExclusivities)
        {
            if (!IsSpotProductASponsor(product, sponsoredProductsExternalRefs))
            {
                return;
            }

            bool doesBreakAlreadyContainACompetitorSpotByClashCode = DoesBreakAlreadyContainACompetitorSpotByClashCode(
                spotsAlreadyInTheBreak,
                clashExclusivities);

            bool doesBreakAlreadyContainACompetitorSpotByAdvertiser = DoesBreakAlreadyContainACompetitorSpotByAdvertiserCode(
                spotsAlreadyInTheBreak,
                advertiserExclusivities);

            if (doesBreakAlreadyContainACompetitorSpotByClashCode)
            {
                AddCompetitorByClashRestrictionFailure(spotUid, result);
            }

            if (doesBreakAlreadyContainACompetitorSpotByAdvertiser)
            {
                AddCompetitorByAdvertiserRestrictionFailure(spotUid, result);
            }
        }

        /// <summary>
        /// <para>
        /// If a break contains a sponsored spot do not place any competitor
        /// spots in the break.
        /// </para>
        /// <para>
        /// This is the basis of most of the combinations so extract it to a
        /// common place.
        /// </para>
        /// </summary>
        private static void AddFailureIfSpotIsACompetitorAndBreakAlreadyContainsASponsoredSpot(
            Guid spotUid,
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            ICollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result,
            IReadOnlyCollection<string> sponsoredProductsExternalRefs,
            bool isThisSpotACompetitorByClashCode,
            bool isThisSpotACompetitorByAdvertiser)
        {
            bool doesBreakAlreadyContainASponsoredSpot = DoesBreakAlreadyContainASponsoredSpot(
                spotsAlreadyInTheBreak,
                sponsoredProductsExternalRefs);

            if (!doesBreakAlreadyContainASponsoredSpot)
            {
                return;
            }

            if (isThisSpotACompetitorByClashCode)
            {
                AddCompetitorByClashRestrictionFailure(spotUid, result);
            }

            if (isThisSpotACompetitorByAdvertiser)
            {
                AddCompetitorByAdvertiserRestrictionFailure(spotUid, result);
            }
        }

        private static bool DoesBreakAlreadyContainASponsoredSpot(
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            IReadOnlyCollection<string> sponsoredProductsExternalRefs)
        {
            return spotsAlreadyInTheBreak.Any(s =>
                IsSpotProductASponsor(s.Product, sponsoredProductsExternalRefs)
                );
        }

        private bool DoesBreakAlreadyContainACompetitorSpotByClashCode(
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            IReadOnlyCollection<ClashExclusivity> clashExclusivities)
        {
            foreach (Spot spot in spotsAlreadyInTheBreak)
            {
                if (IsThisACompetitorByClashCode(spot.Uid, clashExclusivities))
                {
                    return true;
                }
            }

            return false;
        }

        private bool DoesBreakAlreadyContainACompetitorSpotByAdvertiserCode(
            IReadOnlyCollection<Spot> spotsAlreadyInTheBreak,
            IReadOnlyCollection<AdvertiserExclusivity> advertiserExclusivities)
        {
            foreach (Spot spot in spotsAlreadyInTheBreak)
            {
                if (IsThisACompetitorByAdvertiserCode(spot.Uid, advertiserExclusivities))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSpotProductASponsor(
            string productExternalRef,
            IReadOnlyCollection<string> sponsoredProductsExternalRefs
            ) => sponsoredProductsExternalRefs.Contains(productExternalRef);

        private bool IsThisACompetitorByClashCode(
            Guid spotUid,
            IReadOnlyCollection<ClashExclusivity> clashExclusivities)
        {
            if (clashExclusivities.Count == 0)
            {
                return false;
            }

            SpotInfo spotInfo = _spotInfos[spotUid];

            (string childClashCode, string parentClashCode) spotProductClashCodes = (
                childClashCode: spotInfo.ProductClashCode,
                parentClashCode: spotInfo.ParentProductClashCode
                );

            return clashExclusivities
                .Any(ce => spotProductIsACompetitor(ce.ClashExternalRef));

            bool spotProductIsACompetitor(string clashExternalRef) =>
                clashExternalRef == spotProductClashCodes.childClashCode
                || clashExternalRef == spotProductClashCodes.parentClashCode;
        }

        private bool IsThisACompetitorByAdvertiserCode(
            Guid spotUid,
            IEnumerable<AdvertiserExclusivity> advertiserExclusivities)
        {
            var productAdvertiserId = _spotInfos[spotUid].ProductAdvertiserIdentifier;

            return advertiserExclusivities
                .Any(c => c.AdvertiserIdentifier == productAdvertiserId);
        }

        /// <summary>
        /// Force a recalculation of restriction limits when a spot is added or removed.
        /// </summary>
        /// <param name="spotAction">The action performed.</param>
        /// <param name="spot">
        /// The spot that was added or removed from the break.
        /// </param>
        /// <param name="theBreak">
        /// A reference to the break the spot was added to or removed from.
        /// </param>
        public virtual void TriggerRecalculationOfAllowedRestrictionLimits(
             SpotAction spotAction,
             Spot spot,
             Break theBreak)
        {
            IEnumerable<(bool found, SpotToSponsorshipRelationship relationship)> spotToSponsorships =
                FindSpotToRestrictionRelationship(
                    spot.Uid,
                    spot.Product,
                    theBreak.ScheduledDate,
                    theBreak.Duration);
            foreach (var spotToSponsorship in spotToSponsorships)
            {
                if (!spotToSponsorship.found)
                {
                    continue;
                }

                if (spotToSponsorship.relationship.CalculationType == SponsorshipCalculationType.None ||
                    spotToSponsorship.relationship.CalculationType == SponsorshipCalculationType.Exclusive)
                {
                    return;
                }

                switch (spotAction)
                {
                    case SpotAction.NoAction:
                        return;

                    case SpotAction.AddSpot:
                        /*
                         * Was the spot a
                         *  1) Sponsor (so when we need to calculate how many competitors we can have [count|duration][flat|percent][timeline|programme])
                         *  2) Competitor (so we know how many competitors we added in total [count|duration][flat|percent][timeline|programme])
                         *  3) Both
                         *  4) Neither (don't raise the event)
                         */

                        AddedSpotToBreakEventArgs eventArgs;

                        if (spotToSponsorship.relationship.IsSponsorSpot)
                        {
                            eventArgs = CreateAddSponsoredSpotEventArg();
                        }
                        else
                        {
                            switch (spotToSponsorship.relationship.CompetitorType)
                            {
                                case SponsorshipCompetitorType.Neither:
                                    return;

                                case SponsorshipCompetitorType.Advertiser:
                                case SponsorshipCompetitorType.Clash:
                                case SponsorshipCompetitorType.Both:
                                    eventArgs = CreateAddCompetitorSpotEventArg();
                                    break;

                                default:
                                    throw new InvalidOperationException(
                                        $"Unimplemented value {nameof(spotToSponsorship.relationship.CompetitorType)} for {nameof(SponsorshipCompetitorType)}"
                                        );
                            }
                        }

                        OnRaiseAddedSpotToBreakEvent(eventArgs);

                        break;

                    case SpotAction.RemoveSpot:
                        OnRaiseRemovedSpotFromBreakEvent(CreateRemoveSpotEventArg());

                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(spotAction));
                }

                RemovedSpotFromBreakEventArgs CreateRemoveSpotEventArg()
                {
                    return spotToSponsorship.relationship.IsSponsorSpot ?
                        CreateRemoveSponsoredSpotEventArg() :
                        CreateRemoveCompetitorSpotEventArg();
                }
                RemovedSpotFromBreakEventArgs CreateRemoveCompetitorSpotEventArg()
                {
                    if (spotToSponsorship.relationship.RestrictionType ==
                        SponsorshipRestrictionType.SpotDuration)
                    {
                        return new RemovedCompetitorSpotDurationFromBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            _spotInfos[spot.Uid].ProductAdvertiserIdentifier,
                            _spotInfos[spot.Uid].ProductClashCode,
                            spot.Product,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType,
                            spot.SpotLength
                            );
                    }
                    else
                    {
                        return new RemovedCompetitorSpotCountFromBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            _spotInfos[spot.Uid].ProductAdvertiserIdentifier,
                            _spotInfos[spot.Uid].ProductClashCode,
                            spot.Product,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType
                            );
                    }
                }

                RemovedSpotFromBreakEventArgs CreateRemoveSponsoredSpotEventArg()
                {
                    if (spotToSponsorship.relationship.RestrictionType ==
                        SponsorshipRestrictionType.SpotDuration)
                    {
                        return new RemovedSponsoredSpotDurationFromBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            spot.Product,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType,
                            spot.SpotLength
                            );
                    }
                    else
                    {
                        return new RemovedSponsoredSpotCountFromBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            spot.Product,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType
                            );
                    }
                }

                AddedSpotToBreakEventArgs CreateAddCompetitorSpotEventArg()
                {
                    if (spotToSponsorship.relationship.RestrictionType ==
                        SponsorshipRestrictionType.SpotDuration)
                    {
                        return new AddedCompetitorSpotDurationToBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            spot.Product,
                            _spotInfos[spot.Uid].ProductAdvertiserIdentifier,
                            _spotInfos[spot.Uid].ProductClashCode,
                            spotToSponsorship.relationship.CompetitorType,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType,
                            spot.SpotLength
                            );
                    }
                    else
                    {
                        return new AddedCompetitorSpotCountToBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            spot.Product,
                            _spotInfos[spot.Uid].ProductAdvertiserIdentifier,
                            _spotInfos[spot.Uid].ProductClashCode,
                            spotToSponsorship.relationship.CompetitorType,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType
                            );
                    }
                }

                AddedSpotToBreakEventArgs CreateAddSponsoredSpotEventArg()
                {
                    if (spotToSponsorship.relationship.RestrictionType ==
                        SponsorshipRestrictionType.SpotDuration)
                    {
                        return new AddedSponsoredSpotDurationToBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            spot.Product,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType,
                            spot.SpotLength
                            );
                    }
                    else
                    {
                        return new AddedSponsoredSpotCountToBreakEventArgs(
                            theBreak.ExternalBreakRef,
                            spot.Product,
                            spotToSponsorship.relationship.Applicability,
                            spotToSponsorship.relationship.CalculationType);
                    }
                }
            }
        }

        private IEnumerable<(bool found, SpotToSponsorshipRelationship relationship)>
            FindSpotToRestrictionRelationship(
            Guid spotUid,
            string productExternalReference,
            DateTime breakScheduledDate,
            Duration breakDuration)
        {
            IEnumerable<SponsoredItem> matchingSponsoredItems = GetMatchingSponsoredItems(breakScheduledDate, breakDuration);

            if (!matchingSponsoredItems.Any())
            {
                yield return (false, new SpotToSponsorshipRelationship());
            }

            foreach (var matchingSponsoredItem in matchingSponsoredItems)
            {
                IReadOnlyCollection<string> sponsoredProductsExternalRefs = matchingSponsoredItem.Products.ToList() ?? new List<string>(0);
                IReadOnlyCollection<ClashExclusivity> clashExclusivities = matchingSponsoredItem.ClashExclusivities ?? new List<ClashExclusivity>(0);
                IReadOnlyCollection<AdvertiserExclusivity> advertiserExclusivities = matchingSponsoredItem.AdvertiserExclusivities ?? new List<AdvertiserExclusivity>(0);

                SponsorshipCompetitorType competitorType = SponsorshipCompetitorType.Neither;

                bool isSponsorSpot = false;
                if (IsSpotProductASponsor(productExternalReference, sponsoredProductsExternalRefs))
                {
                    isSponsorSpot = true;
                }
                else
                {
                    competitorType = GetCompetitorType();
                }

                SponsorshipApplicability applicability = matchingSponsoredItem.Applicability ?? SponsorshipApplicability.AllCompetitors;

                SponsorshipRestrictionType restrictionType = GetRestrictionType(
                    matchingSponsoredItem.RestrictionType,
                    applicability,
                    clashExclusivities,
                    advertiserExclusivities);

                SponsorshipCalculationType calculationType = matchingSponsoredItem.CalculationType;

                yield return (true, new SpotToSponsorshipRelationship(
                    isSponsorSpot,
                    applicability,
                    competitorType,
                    restrictionType,
                    calculationType)
                );

                SponsorshipCompetitorType GetCompetitorType()
                {
                    if (IsThisACompetitorByAdvertiserCode(spotUid, advertiserExclusivities))
                    {
                        competitorType = SponsorshipCompetitorType.Advertiser;
                    }

                    if (IsThisACompetitorByClashCode(spotUid, clashExclusivities))
                    {
                        competitorType =
                            competitorType == SponsorshipCompetitorType.Advertiser ?
                            SponsorshipCompetitorType.Both :
                            SponsorshipCompetitorType.Clash;
                    }

                    return competitorType;
                }
            }
        }

        private IEnumerable<SponsoredItem> GetMatchingSponsoredItems(
            DateTime breakScheduledDate,
            Duration breakDuration)
        {
            var breakStartTime = breakScheduledDate.TimeOfDay;
            var breakEndTime = breakStartTime.Add(breakDuration.ToTimeSpan());

            return _sponsorshipRestrictions
                .SelectMany(s => s.SponsoredItems)
                .Where(item =>
                    GetRestrictionsForThisBreak(item, breakStartTime, breakEndTime).Count > 0);
        }

        private static SponsorshipRestrictionType GetRestrictionType(
            SponsorshipRestrictionType? restrictionType,
            SponsorshipApplicability applicability,
            IReadOnlyCollection<ClashExclusivity> clashExclusivities,
            IReadOnlyCollection<AdvertiserExclusivity> advertiserExclusivities)
        {
            switch (applicability)
            {
                case SponsorshipApplicability.AllCompetitors:
                    return GetRestrictionOrDefault();

                case SponsorshipApplicability.EachCompetitor:
                    return GetRestrictionTypeByEachCompetitor();

                default:
                    return SponsorshipRestrictionType.SpotCount;
            }

            SponsorshipRestrictionType GetRestrictionOrDefault() =>
                restrictionType ?? SponsorshipRestrictionType.SpotCount;

            SponsorshipRestrictionType GetRestrictionTypeByEachCompetitor()
            {
                if (advertiserExclusivities.Count > 0)
                {
                    return GetAdvertiserRestrictionOrDefault();
                }
                else if (clashExclusivities.Count > 0)
                {
                    return GetClashRestrictionOrDefault();
                }

                return SponsorshipRestrictionType.SpotCount;
            }

            SponsorshipRestrictionType GetAdvertiserRestrictionOrDefault() =>
                advertiserExclusivities.First().RestrictionType ??
                    SponsorshipRestrictionType.SpotCount;

            SponsorshipRestrictionType GetClashRestrictionOrDefault() =>
                clashExclusivities.First().RestrictionType ??
                    SponsorshipRestrictionType.SpotCount;
        }

        private static void AddCompetitorByClashRestrictionFailure(
            Guid spotUid,
            ICollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result)
        {
            result.Add(
                (spotUid, SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash)
                );
        }

        private static void AddCompetitorByAdvertiserRestrictionFailure(
            Guid spotUid,
            ICollection<(Guid spotUid, SmoothFailureMessages failureMessage)> result)
        {
            result.Add(
                (spotUid, SmoothFailureMessages.T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser)
                );
        }
    }
}
