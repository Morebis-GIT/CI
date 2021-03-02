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
        /// <para>For debugging only.</para>
        /// <para>Record why the method returned to the caller. Note, this is
        /// different from the exit code returned to the caller. It is for
        /// tracking where in the method the return occurred as the same exit
        /// code can be returned in several places.</para>
        /// </summary>
        /// <remarks>
        /// The <see cref="Guid"/> is the unique identifier for the restriction
        /// that caused the exit reason.
        /// </remarks>
        public IReadOnlyList<(Guid restrictionId, DebugExitReason_Restriction exitReason)> ExitReasons =>
            _exitReasons;

        private readonly IImmutableList<Restriction> _restrictions;
        private readonly IEnumerable<Product> _products;
        private readonly IEnumerable<Clash> _clashes;
        private readonly IImmutableList<IndexType> _indexTypes;
        private readonly IImmutableList<Universe> _universes;
        private readonly IImmutableList<RatingsPredictionSchedule> _ratingsPredictionSchedules;

        private static readonly List<CheckRestrictionResult> _emptyCheckRestrictionResults =
            Enumerable
                .Empty<CheckRestrictionResult>()
                .ToList();

        /// <summary>
        /// <para>For debugging only.</para>
        /// <para>
        /// The reason why a restriction check was terminated.
        /// </para>
        /// </summary>
        private readonly List<(Guid, DebugExitReason_Restriction)> _exitReasons =
            new List<(Guid, DebugExitReason_Restriction)>();

        /// <summary>
        /// <para>
        /// Declares a class for checking which, if any, restrictions apply to
        /// <see cref="Spot"/> placement.
        /// </para>
        /// </summary>
        /// <param name="restrictionsToCheck"></param>
        public RestrictionChecker(
            IImmutableList<Restriction> restrictionsToCheck,
            IEnumerable<Product> products,
            IEnumerable<Clash> clashes,
            IImmutableList<IndexType> indexTypes,
            IImmutableList<Universe> universes,
            IImmutableList<RatingsPredictionSchedule> ratingsPredictionSchedules)
        {
            _restrictions = restrictionsToCheck ?? ImmutableList<Restriction>.Empty;
            _products = products;
            _clashes = clashes;
            _indexTypes = indexTypes;
            _universes = universes;
            _ratingsPredictionSchedules = ratingsPredictionSchedules;
        }

        /// <summary>
        /// <para>
        /// Get the reasons why the <see cref="Spot"/> has restrictions with the given
        /// <see cref="Break"/>.
        /// </para>
        /// </summary>
        /// <param name="theBreak">The <see cref="Break"/> instance must match the
        /// restrictions to be considered. If the Break object is <c>null</c> it
        /// will be treated as if no restrictions matched.</param>
        /// <param name="salesArea">The sales area's school and public holidays
        /// can be considered during restriction checking. A <c>null</c>
        /// <see cref="SalesArea"/> object will throw an
        /// <see cref="ArgumentNullException"/>.</param>
        /// <param name="breaksForThePeriodBeingSmoothed">All of the breaks for
        /// the period being smoothed, for example, a whole week.</param>
        public List<CheckRestrictionResult> CheckRestrictions(
            Programme programme,
            Break theBreak,
            Spot spot,
            SalesArea salesArea,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            if (salesArea is null)
            {
                // We shouldn't treat a null sales area object as if we intended
                // not to pass holidays.
                throw new ArgumentNullException(nameof(salesArea));
            }

            if (_restrictions.Count == 0)
            {
                AddExitReason(Guid.Empty, DebugExitReason_Restriction.NoRestrictionsToCheck);
                return _emptyCheckRestrictionResults;
            }

            if (theBreak is null)
            {
                // None of the restrictions will match a null break.
                // Trap this here as there's no point trapping it in each
                // loop iteration.
                AddExitReason(Guid.Empty, DebugExitReason_Restriction.NoRestrictionsToCheck);
                return _emptyCheckRestrictionResults;
            }

            var results = new List<CheckRestrictionResult>();

            foreach (var restriction in _restrictions)
            {
                var result = CheckRestriction(
                    restriction,
                    spot,
                    theBreak,
                    programme,
                    breaksForThePeriodBeingSmoothed,
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

        private void AddExitReason(
            Guid restrictionId,
            DebugExitReason_Restriction exitReason
            )
        {
            _exitReasons.Add((restrictionId, exitReason));
        }

        /// <summary>
        /// <para>
        /// Indicates whether the spot placement in this break is prevented by
        /// the restriction.
        /// </para>
        /// <para>
        /// If the break does not match the restriction criteria the spot will
        /// not be checked and a restriction reason of
        /// <see cref="RestrictionReasons.None"/> is returned.
        /// </para>
        /// </summary>
        /// <param name="theBreak">The break's sales area and schedule dates must
        /// match the restriction criteria to be considered. Must not be
        /// <c>null</c>.</param>
        private RestrictionReasons CheckRestriction(
            Restriction restriction,
            Spot spot,
            Break theBreak,
            Programme programme,
            IReadOnlyCollection<Break> breaksForThePeriodBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            IReadOnlyCollection<DateRange> schoolHolidays,
            IReadOnlyCollection<DateRange> publicHolidays
            )
        {
            if (restriction.SalesAreas?.Count > 0 &&
                !restriction.SalesAreas.Contains(theBreak.SalesArea))
            {
                AddExitReason(
                    restriction.Uid,
                    DebugExitReason_Restriction.IsNotForBreakSalesArea
                    );
                return RestrictionReasons.None;
            }

            var restrictionWindow = new RestrictionWindow(
                (restriction.StartDate, restriction.EndDate),
                (restriction.StartTime, restriction.EndTime)
            );

            if (!restrictionWindow.Contains(theBreak.ScheduledDate))
            {
                AddExitReason(
                    restriction.Uid,
                    DebugExitReason_Restriction.StartEndDateTimeDoesNotContainTheBreak
                    );
                return RestrictionReasons.None;
            }

            if (!restriction.RestrictionDaysOfWeek.Contains(theBreak.ScheduledDate.DayOfWeek))
            {
                AddExitReason(
                    restriction.Uid,
                    DebugExitReason_Restriction.DoesNotCoverTheBreakDay
                    );
                return RestrictionReasons.None;
            }

            if (restriction.SchoolHolidayIndicator != IncludeOrExcludeOrEither.X)
            {
                bool breakIsInSchoolHoliday = IsBreakDuringHolidayPeriod(theBreak, schoolHolidays);

                switch (restriction.SchoolHolidayIndicator)
                {
                    case IncludeOrExcludeOrEither.I when !breakIsInSchoolHoliday:
                        AddExitReason(
                            restriction.Uid,
                            DebugExitReason_Restriction.AppliesToBreaksWithinSchoolHolidays
                            );
                        return RestrictionReasons.None;

                    case IncludeOrExcludeOrEither.E when breakIsInSchoolHoliday:
                        AddExitReason(
                            restriction.Uid,
                            DebugExitReason_Restriction.AppliesToBreaksOutsideSchoolHolidays
                            );
                        return RestrictionReasons.None;
                }
            }

            if (restriction.PublicHolidayIndicator != IncludeOrExcludeOrEither.X)
            {
                bool breakIsInPublicHoliday = IsBreakDuringHolidayPeriod(theBreak, publicHolidays);

                switch (restriction.PublicHolidayIndicator)
                {
                    case IncludeOrExcludeOrEither.I when !breakIsInPublicHoliday:
                        AddExitReason(
                            restriction.Uid,
                            DebugExitReason_Restriction.AppliesToBreaksWithinPublicHolidays
                            );
                        return RestrictionReasons.None;

                    case IncludeOrExcludeOrEither.E when breakIsInPublicHoliday:
                        AddExitReason(
                            restriction.Uid,
                            DebugExitReason_Restriction.AppliesToBreaksOutsidePublicHolidays
                            );
                        return RestrictionReasons.None;
                }
            }

            if (restriction.RestrictionType != RestrictionType.Time && IsExcludedFromLiveProgramme())
            {
                AddExitReason(
                    restriction.Uid,
                    DebugExitReason_Restriction.DoesNotApplyToLiveBroadcasts
                    );
                return RestrictionReasons.None;
            }

            var result = RestrictionReasons.None;

            switch (restriction.RestrictionBasis)
            {
                case RestrictionBasis.Clash:
                    {
                        Product product = _products
                            .FirstOrDefault(p => p.Externalidentifier == spot.Product);

                        Clash clash = product is null
                            ? null
                            : _clashes.FirstOrDefault(c => c.Externalref == product.ClashCode);

                        if (clash is null)
                        {
                            AddExitReason(
                                restriction.Uid,
                                DebugExitReason_Restriction.BasisIsByClashButNoClashFound
                                );
                            return RestrictionReasons.None;
                        }
                        else if (restriction.ClashCode == clash.Externalref ||
                            restriction.ClashCode == clash.ParentExternalidentifier)
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
                        // Smooth checks spot clearance code.
                        // Optimiser checks campaign clearance code.
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

            bool doesRestrictionTypeApply = false;
            switch (restriction.RestrictionType)
            {
                case RestrictionType.Index:
                    IndexType indexType = _indexTypes.FirstOrDefault(it => it.CustomId == restriction.IndexType);

                    Universe universeBase = _universes.FirstOrDefault(u =>
                        u.SalesArea == spot.SalesArea &&
                        u.Demographic == indexType.BaseDemographicNo &&
                        theBreak.ScheduledDate.Date >= u.StartDate.Date &&
                        theBreak.ScheduledDate.Date <= u.EndDate.Date);

                    if (universeBase is null)
                    {
                        throw new Exception(string.Format("Base universe for restriction index type {0} for demographic {1} not found", restriction.IndexType.ToString(), indexType.BaseDemographicNo));
                    }

                    Universe universe = _universes.FirstOrDefault(u =>
                        u.SalesArea == spot.SalesArea &&
                        u.Demographic == indexType.DemographicNo &&
                        theBreak.ScheduledDate.Date >= u.StartDate.Date &&
                        theBreak.ScheduledDate.Date <= u.EndDate.Date);

                    if (universe is null)
                    {
                        throw new Exception(string.Format("Universe for restriction index type {0} for demographic {1} not found", restriction.IndexType.ToString(), indexType.DemographicNo));
                    }

                    // Proportion in multi-channel universe
                    double proportionInUniverse = universe.UniverseValue / (double)universeBase.UniverseValue;

                    // Get ratings
                    var ratingsPredictionSchedule = _ratingsPredictionSchedules.FirstOrDefault(r => r.SalesArea == spot.SalesArea && r.ScheduleDay.Date == theBreak.ScheduledDate.Date);

                    var ratingsBase = ratingsPredictionSchedule.Ratings.Find(r => r.Demographic == universeBase.Demographic && r.Time == BreakUtilities.GetRatingScheduleDateTimeForBreak(theBreak.ScheduledDate, TimeSpan.FromMinutes(15)));
                    var ratings = ratingsPredictionSchedule.Ratings.Find(r => r.Demographic == universe.Demographic && r.Time == BreakUtilities.GetRatingScheduleDateTimeForBreak(theBreak.ScheduledDate, TimeSpan.FromMinutes(15)));

                    if (proportionInUniverse > 0 && ratingsBase.NoOfRatings > 0)
                    {
                        // Proportion watching break
                        double proportionWatchingBreak = ratings.NoOfRatings / ratingsBase.NoOfRatings;

                        // Calculate index
                        int index = (int)(100 * (proportionWatchingBreak / proportionInUniverse));

                        if (restriction.IndexThreshold >= index)     // Exceeds threshold
                        {
                            doesRestrictionTypeApply = true;
                        }
                    }
                    break;

                case RestrictionType.Programme:
                    {
                        IReadOnlyList<Programme> restrictionProgrammes = GetProgrammeAndOneEitherSideOfIt(
                            restriction,
                            programme,
                            breaksForThePeriodBeingSmoothed,
                            scheduleProgrammes);

                        // Tolerance applies
                        if (restrictionProgrammes.Any(rp => rp.ExternalReference == restriction.ExternalProgRef))
                        {
                            doesRestrictionTypeApply = true;
                        }
                        break;
                    }

                case RestrictionType.ProgrammeCategory:
                    {
                        IReadOnlyList<Programme> restrictionProgrammes = GetProgrammeAndOneEitherSideOfIt(
                            restriction,
                            programme,
                            breaksForThePeriodBeingSmoothed,
                            scheduleProgrammes);

                        // Tolerance applies
                        if (restrictionProgrammes.Any(rp => rp.ProgrammeCategories.Contains(restriction.ProgrammeCategory)))
                        {
                            doesRestrictionTypeApply = true;
                        }
                        break;
                    }

                case RestrictionType.ProgrammeClassification:
                    // Tolerance doesn't apply, only need to check current programme
                    switch (restriction.ProgrammeClassificationIndicator)
                    {
                        case IncludeOrExclude.E:
                            doesRestrictionTypeApply = restriction.ProgrammeClassification != programme.Classification;
                            break;

                        case IncludeOrExclude.I:
                            doesRestrictionTypeApply = restriction.ProgrammeClassification == programme.Classification;
                            break;
                    }
                    break;

                case RestrictionType.Time:
                    doesRestrictionTypeApply = true;
                    break;
            }

            AddExitReason(
                restriction.Uid,
                DebugExitReason_Restriction.EndOfMethod
                );

            return doesRestrictionTypeApply ? result : RestrictionReasons.None;

            //----------------------------------------
            // Local functions
            bool IsExcludedFromLiveProgramme() =>
                restriction.LiveProgrammeIndicator == IncludeOrExclude.E &&
                programme.LiveBroadcast;

            static bool IsBreakDuringHolidayPeriod(
                Break aBreak,
                IReadOnlyCollection<DateRange> holidayPeriod)
            {
                if (aBreak is null ||
                    holidayPeriod is null)
                {
                    return false;
                }

                var breakDate = aBreak.ScheduledDate.Date;

                return holidayPeriod.Any(holiday =>
                    holiday.Contains(breakDate)
                    );
            }
        }

        /// <summary>Gets the programme and one either side of it based on the
        /// restriction's tolerences</summary>
        /// <param name="restriction">The restriction.</param>
        /// <param name="programme">The programme.</param>
        /// <param name="breaksBeingSmoothed">The breaks being smoothed.</param>
        /// <param name="scheduleProgrammes">The schedule programmes.</param>
        /// <returns></returns>
        private static List<Programme> GetProgrammeAndOneEitherSideOfIt(
            Restriction restriction,
            Programme programme,
            IReadOnlyCollection<Break> breaksBeingSmoothed,
            IReadOnlyCollection<Programme> scheduleProgrammes)
        {
            var programmeStartPlusTolerance = programme.StartDateTime.Add(TimeSpan.FromMinutes(-restriction.TimeToleranceMinsBefore));
            var programmeEndPlusTolerance = programme.StartDateTime.Add(programme.Duration.ToTimeSpan()).Add(TimeSpan.FromMinutes(restriction.TimeToleranceMinsAfter));

            var restrictionBreaks = breaksBeingSmoothed
                .Where(b => b.ScheduledDate >= programmeStartPlusTolerance
                        && b.ScheduledDate <= programmeEndPlusTolerance)
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

            return restrictionProgrammes;
        }
    }
}
