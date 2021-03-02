using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class RestrictionChecker
        : IRestrictionChecker
    {
        /// <summary>
        /// <para>Record why the method returned to the caller. Note, this is
        /// different from the exit code returned to the caller. It is for
        /// tracking where in the method the return occurred as the same exit
        /// code can be returned in several places.</para>
        /// <para>For debugging only.</para>
        /// </summary>
        /// <param name="value"></param>
        public DebugRestrictionCheckerExitReason ExitReason { get; private set; }
            = DebugRestrictionCheckerExitReason.ValueHasNotBeenSet;

        private readonly IImmutableList<Restriction> _restrictions;
        private readonly IImmutableList<Product> _products;
        private readonly IImmutableList<Clash> _clashes;
        private readonly IImmutableList<IndexType> _indexTypes;
        private readonly IImmutableList<Universe> _universes;
        private readonly IImmutableList<RatingsPredictionSchedule> _ratingsPredictionSchedules;

        /// <summary>
        /// Declares a class for checking which, if any, restrictions apply to
        /// <see cref="Spot"/> placement.
        /// </summary>
        public RestrictionChecker(
            IImmutableList<Restriction> restrictions,
            IImmutableList<Product> products,
            IImmutableList<Clash> clashes,
            IImmutableList<IndexType> indexTypes,
            IImmutableList<Universe> universes,
            IImmutableList<RatingsPredictionSchedule> ratingsPredictionSchedules)
        {
            _restrictions = restrictions;
            _products = products;
            _clashes = clashes;
            _indexTypes = indexTypes;
            _universes = universes;
            _ratingsPredictionSchedules = ratingsPredictionSchedules;
        }

        /// <summary>
        /// Gets all restrictions that the spot conflicts with.
        /// </summary>
        public List<CheckRestrictionResult> CheckRestrictions(
            Programme programme,
            Break oneBreak,
            Spot spot,
            SalesArea salesArea,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            if (_restrictions.Count == 0)
            {
                ExitReason = DebugRestrictionCheckerExitReason.NoRestrictionsToCheck;
                return Enumerable.Empty<CheckRestrictionResult>().ToList();
            }

            var results = new List<CheckRestrictionResult>();

            foreach (var restriction in _restrictions)
            {
                var result = CheckRestriction(
                    programme,
                    oneBreak,
                    spot,
                    restriction,
                    breaksBeingSmoothed,
                    scheduleProgrammes,
                    salesArea.SchoolHolidays,
                    salesArea.PublicHolidays
                    );

                if (result == RestrictionReasons.None)
                {
                    continue;
                }

                results.Add(new CheckRestrictionResult(restriction, result));
            }

            return results;
        }

        /// <summary>
        /// Indicates whether the spot placement is prevented by the restriction.
        /// </summary>
        private RestrictionReasons CheckRestriction(
            Programme programme,
            Break aBreak,
            Spot spot,
            Restriction restriction,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            IReadOnlyCollection<DateRange> schoolHolidays,
            IReadOnlyCollection<DateRange> publicHolidays
            )
        {
            if (restriction.SalesAreas?.Count > 0 && !restriction.SalesAreas.Contains(aBreak.SalesArea))
            {
                ExitReason = DebugRestrictionCheckerExitReason.RestrictionIsNotForBreakSalesArea;
                return RestrictionReasons.None;
            }

            var restrictionWindow = new RestrictionWindow(
                (restriction.StartDate, restriction.EndDate),
                (restriction.StartTime, restriction.EndTime)
            );

            if (!restrictionWindow.Contains(aBreak.ScheduledDate))
            {
                ExitReason = DebugRestrictionCheckerExitReason.BreakIsOutsideTheRestrictionStartEndDateTime;
                return RestrictionReasons.None;
            }

            if (!restriction.RestrictionDaysOfWeek.Contains(aBreak.ScheduledDate.DayOfWeek))
            {
                ExitReason = DebugRestrictionCheckerExitReason.RestrictionDoesNotCoverTheBreakDay;
                return RestrictionReasons.None;
            }

            if (restriction.SchoolHolidayIndicator != IncludeOrExcludeOrEither.X)
            {
                bool isSchoolHoliday = schoolHolidays.Any(sh =>
                    new DateTimeRange(sh.Start.Date, sh.End.Date).Contains(aBreak.ScheduledDate.Date)
                    );

                switch (restriction.SchoolHolidayIndicator)
                {
                    case IncludeOrExcludeOrEither.I when !isSchoolHoliday:
                        ExitReason = DebugRestrictionCheckerExitReason.RestrictionOnlyAppliesToSchoolHolidays;
                        return RestrictionReasons.None;

                    case IncludeOrExcludeOrEither.E when isSchoolHoliday:
                        return RestrictionReasons.None;
                }
            }

            bool isPublicHoliday = publicHolidays.Any(sh =>
                new DateTimeRange(sh.Start.Date, sh.End.Date).Contains(aBreak.ScheduledDate.Date)
                );

            switch (restriction.PublicHolidayIndicator)
            {
                case IncludeOrExcludeOrEither.X:
                    break;

                case IncludeOrExcludeOrEither.I:    // Restriction applies to public holidays
                    if (!isPublicHoliday)           // Doesn't apply, not a public holiday, restriction only applies to public holidays
                    {
                        return RestrictionReasons.None;
                    }
                    break;

                case IncludeOrExcludeOrEither.E: // Restriction applies outside of public holidays
                    if (isPublicHoliday)        // Doesn't apply, public holiday, restriction only applies outside of public holidays
                    {
                        return RestrictionReasons.None;
                    }
                    break;
            }

            // Check live programme indicator Live broadcast flag is not valid
            // for time based restrictions
            if (restriction.RestrictionType != RestrictionType.Time && IsExcludedFromLiveProgramme())
            {
                return RestrictionReasons.None;
            }

            // First, check restriction basis for spot, determines return value
            // for this method if the restriction is valid
            var result = RestrictionReasons.None;

            switch (restriction.RestrictionBasis)
            {
                case RestrictionBasis.Clash:
                    {
                        Product product = _products.FirstOrDefault(p => p.Externalidentifier == spot.Product);
                        Clash clash = product is null ? null : _clashes.FirstOrDefault(c => c.Externalref == product.ClashCode);

                        if (clash is null)
                        {
                            return RestrictionReasons.None;
                        }
                        else if (restriction.ClashCode == clash.Externalref || restriction.ClashCode == clash.ParentExternalidentifier)  // Restricted
                        {
                            result = restriction.RestrictionType switch
                            {
                                RestrictionType.Time => RestrictionReasons.TimeRestrictionForClash,
                                RestrictionType.Programme => RestrictionReasons.ProgrammeRestrictionForClash,
                                RestrictionType.ProgrammeCategory => RestrictionReasons.ProgrammeCategoryRestrictionForClash,
                                RestrictionType.Index => RestrictionReasons.IndexRestrictionForClash,
                                RestrictionType.ProgrammeClassification => RestrictionReasons.ProgrammeClassificationRestrictionForClash,
                                _ => throw new ArgumentOutOfRangeException(nameof(restriction.RestrictionType))
                            };
                        }
                    }
                    break;

                case RestrictionBasis.ClearanceCode:
                    {
                        // Smooth checks spot clearance code, Optimiser checks
                        // campaign clearance code
                        if (restriction.ClearanceCode == spot.ClearanceCode)
                        {
                            result = restriction.RestrictionType switch
                            {
                                RestrictionType.Time => RestrictionReasons.TimeRestrictionForClearanceCode,
                                RestrictionType.Programme => RestrictionReasons.ProgrammeRestrictionForClearanceCode,
                                RestrictionType.ProgrammeCategory => RestrictionReasons.ProgrammeCategoryRestrictionForClearanceCode,
                                RestrictionType.Index => RestrictionReasons.IndexRestrictionForClearanceCode,
                                RestrictionType.ProgrammeClassification => RestrictionReasons.ProgrammeClassificationRestrictionForClearanceCode,
                                _ => throw new ArgumentOutOfRangeException(nameof(restriction.RestrictionType))
                            };
                        }
                    }
                    break;

                case RestrictionBasis.Product:
                    {
                        Product product = _products.FirstOrDefault(p => p.Externalidentifier == spot.Product);
                        if (product is null)
                        {
                            return RestrictionReasons.None;
                        }
                        else if (restriction.ProductCode.ToString() == product.Externalidentifier || restriction.ProductCode.ToString() == product.ParentExternalidentifier)   // Restricted
                        {
                            result = restriction.RestrictionType switch
                            {
                                RestrictionType.Time => RestrictionReasons.TimeRestrictionForProduct,
                                RestrictionType.Programme => RestrictionReasons.ProgrammeRestrictionForProduct,
                                RestrictionType.ProgrammeCategory => RestrictionReasons.ProgrammeCategoryRestrictionForProduct,
                                RestrictionType.Index => RestrictionReasons.IndexRestrictionForProduct,
                                RestrictionType.ProgrammeClassification => RestrictionReasons.ProgrammeClassificationRestrictionForProduct,
                                _ => throw new ArgumentOutOfRangeException(nameof(restriction.RestrictionType))
                            };
                        }
                    }
                    break;
            }

            if (result == RestrictionReasons.None)
            {
                return result;
            }

            // Get all programmes that fall within the programme time and
            // before/after tolerance.
            // TODO: Optimise this, we have all breaks and programmes for the date
            // range. We could consider reducing the list at the top level.
            var restrictionBreaks = breaksBeingSmoothed
                .Where(b => b.ScheduledDate >= programme.StartDateTime.Add(TimeSpan.FromMinutes(-restriction.TimeToleranceMinsBefore))
                        && b.ScheduledDate <= programme.StartDateTime.Add(programme.Duration.ToTimeSpan()).Add(TimeSpan.FromMinutes(restriction.TimeToleranceMinsAfter)))
                .OrderBy(b => b.ScheduledDate);

            var restrictionProgrammes = new List<Programme>();

            foreach (var restrictionBreak in restrictionBreaks)
            {
                IEnumerable<Programme> collection = scheduleProgrammes.Where(sp =>
                    BreakUtilities.IsBreakInProgramme(restrictionBreak, sp)
                    && !restrictionProgrammes.Contains(sp)
                    );

                restrictionProgrammes.AddRange(collection);
            }

            // Check restriction type
            bool isRestrictionTypeApplies = false;
            switch (restriction.RestrictionType)
            {
                case RestrictionType.Index:
                    IndexType indexType = _indexTypes.FirstOrDefault(it => it.CustomId == restriction.IndexType);

                    Universe universeBase = _universes.FirstOrDefault(u =>
                        u.SalesArea == spot.SalesArea &&
                        u.Demographic == indexType.BaseDemographicNo &&
                        aBreak.ScheduledDate.Date >= u.StartDate.Date &&
                        aBreak.ScheduledDate.Date <= u.EndDate.Date);

                    if (universeBase is null)
                    {
                        throw new Exception(string.Format("Base universe for restriction index type {0} for demographic {1} not found", restriction.IndexType.ToString(), indexType.BaseDemographicNo));
                    }

                    Universe universe = _universes.FirstOrDefault(u =>
                        u.SalesArea == spot.SalesArea &&
                        u.Demographic == indexType.DemographicNo &&
                        aBreak.ScheduledDate.Date >= u.StartDate.Date &&
                        aBreak.ScheduledDate.Date <= u.EndDate.Date);

                    if (universe is null)
                    {
                        throw new Exception(string.Format("Universe for restriction index type {0} for demographic {1} not found", restriction.IndexType.ToString(), indexType.DemographicNo));
                    }

                    // Proportion in multi-channel universe
                    double proportionInUniverse = universe.UniverseValue / (double)universeBase.UniverseValue;

                    // Get ratings
                    var ratingsPredictionSchedule = _ratingsPredictionSchedules.FirstOrDefault(r => r.SalesArea == spot.SalesArea && r.ScheduleDay.Date == aBreak.ScheduledDate.Date);

                    var ratingsBase = ratingsPredictionSchedule.Ratings.Find(r => r.Demographic == universeBase.Demographic && r.Time == BreakUtilities.GetRatingScheduleDateTimeForBreak(aBreak.ScheduledDate, TimeSpan.FromMinutes(15)));
                    var ratings = ratingsPredictionSchedule.Ratings.Find(r => r.Demographic == universe.Demographic && r.Time == BreakUtilities.GetRatingScheduleDateTimeForBreak(aBreak.ScheduledDate, TimeSpan.FromMinutes(15)));

                    if (proportionInUniverse > 0 && ratingsBase.NoOfRatings > 0)
                    {
                        // Proportion watching break
                        double proportionWatchingBreak = ratings.NoOfRatings / ratingsBase.NoOfRatings;

                        // Calculate index
                        int index = (int)(100 * (proportionWatchingBreak / proportionInUniverse));

                        if (restriction.IndexThreshold >= index)     // Exceeds threshold
                        {
                            isRestrictionTypeApplies = true;
                        }
                    }
                    break;

                case RestrictionType.Programme:
                    // Tolerance applies
                    if (restrictionProgrammes.Any(rp => rp.ExternalReference == restriction.ExternalProgRef))
                    {
                        isRestrictionTypeApplies = true;
                    }
                    break;

                case RestrictionType.ProgrammeCategory:
                    // Tolerance applies
                    if (restrictionProgrammes.Any(rp => rp.ProgrammeCategories.Contains(restriction.ProgrammeCategory)))
                    {
                        isRestrictionTypeApplies = true;
                    }
                    break;

                case RestrictionType.ProgrammeClassification:
                    // Tolerance doesn't apply, only need to check current programme
                    switch (restriction.ProgrammeClassificationIndicator)
                    {
                        case IncludeOrExclude.E:
                            isRestrictionTypeApplies = restriction.ProgrammeClassification != programme.Classification;
                            break;

                        case IncludeOrExclude.I:
                            isRestrictionTypeApplies = restriction.ProgrammeClassification == programme.Classification;
                            break;
                    }
                    break;

                case RestrictionType.Time:
                    isRestrictionTypeApplies = true;
                    break;
            }

            return isRestrictionTypeApplies ? result : RestrictionReasons.None;

            //----------------------------------------
            // Local functions
            bool IsExcludedFromLiveProgramme() =>
                restriction.LiveProgrammeIndicator == IncludeOrExclude.E &&
                programme.LiveBroadcast;
        }
    }
}
