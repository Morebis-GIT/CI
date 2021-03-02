using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    public class SponsorshipRestrictionFilterService
    {
        private readonly IImmutableList<Sponsorship> _sponsorshipRestrictions;

        public SponsorshipRestrictionFilterService(IImmutableList<Sponsorship> sponsorships)
            => _sponsorshipRestrictions = sponsorships;

        /// <summary>
        /// Filter the list of sponsorship restrictions to those only applicable
        /// for the given programme.
        /// </summary>
        /// <param name="oneProgramme"></param>
        /// <returns></returns>
        public IImmutableList<SponsorshipRestrictionFilterResults>
        Filter(Programme oneProgramme)
        {
            if (_sponsorshipRestrictions is null)
            {
                throw new ArgumentNullException(nameof(_sponsorshipRestrictions));
            }

            if (oneProgramme is null)
            {
                throw new ArgumentNullException(nameof(oneProgramme));
            }

            if (_sponsorshipRestrictions.Count == 0)
            {
                return ImmutableList<SponsorshipRestrictionFilterResults>.Empty;
            }

            string programmeSalesAreaName = oneProgramme.SalesArea;
            string programmeName = oneProgramme.ProgrammeName;
            var programmeStartDateTime = oneProgramme.StartDateTime;
            Duration programmeDuration = oneProgramme.Duration;
            var result = new List<SponsorshipRestrictionFilterResults>();

            foreach (Sponsorship sponsorship in _sponsorshipRestrictions)
            {
                List<SponsoredItem> applicableSponsoredItems = FilterBySalesAreas(
                    programmeSalesAreaName,
                    sponsorship.SponsoredItems);

                applicableSponsoredItems = FilterByStartDateTime(
                        programmeStartDateTime,
                        programmeDuration,
                        applicableSponsoredItems);

                if (sponsorship.RestrictionLevel == SponsorshipRestrictionLevel.Programme)
                {
                    applicableSponsoredItems = FilterByProgrammeName(
                        programmeName,
                        applicableSponsoredItems);
                }

                if (applicableSponsoredItems.Count > 0)
                {
                    result.Add(
                        new SponsorshipRestrictionFilterResults
                        {
                            SponsoredItems = applicableSponsoredItems.ToImmutableList()
                        });
                }
            }

            return result.ToImmutableList();
        }

        /// <summary>
        /// Gets a list of timelines of all the applicable sponsored restrictions
        /// </summary>
        /// <param name="weekBeingSmoothed">The week being smoothed</param>
        /// <param name="salesArea">The sales area</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">salesArea or weekBeingSmoothed</exception>
        public List<SmoothSponsorshipTimeline> GetSponsorshipRestrictionTimeline(
            DateTimeRange weekBeingSmoothed,
            string salesArea)
        {
            var EmptyList = new List<SmoothSponsorshipTimeline>();

            if (salesArea == null)
            {
                throw new ArgumentNullException(nameof(salesArea));
            }

            if (weekBeingSmoothed.Start == DateTime.MinValue ||
                weekBeingSmoothed.End == DateTime.MinValue)
            {
                throw new ArgumentNullException(nameof(weekBeingSmoothed));
            }

            if (_sponsorshipRestrictions is null)
            {
                return EmptyList;
            }

            if (_sponsorshipRestrictions.Count == 0)
            {
                return EmptyList;
            }

            var extractTimeline =
                new TimelineGenerator(
                    weekBeingSmoothed,
                    _sponsorshipRestrictions,
                    salesArea);

            return extractTimeline.ExtractTimelines();
        }

        private static List<SponsoredItem> FilterBy(
            IReadOnlyCollection<SponsoredItem> sponsoredItems,
            Func<SponsorshipItem, bool> expression)
        {
            var applicableSponsoredItems = new List<SponsoredItem>();

            foreach (SponsoredItem sponsoredItem in sponsoredItems)
            {
                var items = sponsoredItem
                    .SponsorshipItems
                    .Where(expression)
                    .ToList();

                if (items.Count > 0)
                {
                    var temp = sponsoredItem.Clone();
                    temp.SponsorshipItems = items;
                    applicableSponsoredItems.Add(temp);
                }
            }

            return applicableSponsoredItems;
        }

        private static List<SponsoredItem> FilterByProgrammeName(
            string programmeName,
            IReadOnlyCollection<SponsoredItem> sponsoredItems
            ) => FilterBy(sponsoredItems,
                s => s.ProgrammeName.Equals(
                    programmeName,
                    StringComparison.InvariantCultureIgnoreCase));

        private static List<SponsoredItem> FilterBySalesAreas(
            string programmeSalesAreaName,
            IReadOnlyCollection<SponsoredItem> sponsoredItems
            ) => FilterBy(sponsoredItems,
                s => s.SalesAreas.Contains(programmeSalesAreaName));

        private static List<SponsoredItem> FilterByStartDateTime(
            DateTime programmeStartDateTime,
            Duration programmeDuration,
            IReadOnlyCollection<SponsoredItem> sponsoredItems)
        {
            return FilterBy(sponsoredItems, s =>
            {
                DateTimeRange sponsoredItemDateRange = (s.StartDate.Date, s.EndDate.Date);

                (DateTime broadcastDate, TimeSpan broadcastTime) =
                DateHelper.ConvertStandardToBroadcast(programmeStartDateTime);

                if (!sponsoredItemDateRange.Contains(broadcastDate))
                {
                    return false;
                }

                TimeSpan programmeStartTime = programmeStartDateTime.TimeOfDay;
                TimeSpan programmeEndTime = programmeStartDateTime.Add(programmeDuration.ToTimeSpan()).TimeOfDay;

                bool programmeHasValidDayPart = false;

                foreach (SponsoredDayPart dayPart in s.DayParts)
                {
                    bool programmeStartsWithinSponsorshipTimeRange =
                       TimeHelper.TimeRangeOverlaps(
                           dayPart.StartTime,
                           dayPart.EndTime,
                           programmeStartTime,
                           programmeEndTime);

                    bool programmeStartsOnCorrectDay =
                        DayOfWeekHelper.DateFallsOnDayOfWeek(
                            dayPart.DaysOfWeek,
                            broadcastDate);

                    programmeHasValidDayPart |=
                        programmeStartsWithinSponsorshipTimeRange &&
                        programmeStartsOnCorrectDay;
                }

                return programmeHasValidDayPart;
            });
        }
    }
}
