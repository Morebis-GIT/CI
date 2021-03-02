using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Creates a timeline using the provided date time range and sales area of
    /// all the sponsorship items, filtering out the sponsorships that are
    /// restricted to programmes into a single list of smooth timelines
    /// </summary>
    public class TimelineGenerator
    {
        private readonly DateTimeRange _dateTimeRange;
        private readonly string _salesArea;
        private readonly IImmutableList<Sponsorship> _sponsorship;

        public TimelineGenerator(
            DateTimeRange dateTimeRange,
            IImmutableList<Sponsorship> sponsorship,
            string salesArea
            )
        {
            _dateTimeRange = dateTimeRange;
            _salesArea = salesArea;
            _sponsorship = sponsorship;
        }

        public List<SmoothSponsorshipTimeline> ExtractTimelines()
        {
            if (_dateTimeRange.Start == DateTime.MinValue ||
                _dateTimeRange.End == DateTime.MinValue)
            {
                throw new ArgumentNullException(nameof(_dateTimeRange));
            }

            if (_sponsorship is null)
            {
                throw new ArgumentNullException(nameof(_sponsorship));
            }

            if (_sponsorship.Count == 0)
            {
                return new List<SmoothSponsorshipTimeline>();
            }

            var sponsorshipItems = ExtractSponsoredItemsBySalesArea();

            if (!sponsorshipItems.Any(s => s.SponsorshipItems.Any()))
            {
                return new List<SmoothSponsorshipTimeline>();
            }

            return CreateTimelinesFromTimeBands(sponsorshipItems);
        }

        /// <summary>
        /// Creates a list of timelines from the provided time bands; only using
        /// valid sponsorships that meet sales area and time stamp
        /// </summary>
        private List<SmoothSponsorshipTimeline> CreateTimelinesFromTimeBands(
            IEnumerable<SponsoredItem> sponsoredItems)
        {
            var smoothTimelines = new List<SmoothSponsorshipTimeline>();

            foreach (var currentDate in _dateTimeRange)
            {
                foreach (var sponsoredItem in sponsoredItems)
                {
                    foreach (var sponsorship in sponsoredItem.SponsorshipItems)
                    {
                        DateTimeRange dateRange = (sponsorship.StartDate, sponsorship.EndDate);
                        foreach (var dayPart in sponsorship.DayParts)
                        {
                            AddSmoothTimelineByDate(
                                sponsoredItem,
                                currentDate,
                                dateRange,
                                dayPart);
                        }
                    }
                }
            }

            return smoothTimelines;

            bool TimeBandIsOnValidDayOfWeek(string[] daysOfWeek, DateTime date) =>
                DayOfWeekHelper.DateFallsOnDayOfWeek(daysOfWeek, date);

            void AddSmoothTimelineByDate(
                SponsoredItem sponsoredItem,
                DateTime currentDate,
                DateTimeRange dateRange,
                SponsoredDayPart dayPart)
            {
                if (!(TimeBandIsOnValidDayOfWeek(dayPart.DaysOfWeek, currentDate) &&
                    dateRange.Contains(currentDate.Date)))
                {
                    return;
                }

                var start = DateHelper.ConvertBroadcastToStandard(currentDate.Date, dayPart.StartTime);
                var end = DateHelper.ConvertBroadcastToStandard(currentDate.Date, dayPart.EndTime);

                List<(string advertiserIdentifier, int restrictionValue)> advertiserExclusivities =
                    GetAdvertiserReferenceAndRestrictionValue(
                        sponsoredItem.AdvertiserExclusivities,
                        sponsoredItem.RestrictionValue ?? 0);

                List<(string clashExternalReference, int restrictionValue)> clashExclusivities =
                    GetClashReferenceAndRestrictionValue(
                        sponsoredItem.ClashExclusivities,
                        sponsoredItem.RestrictionValue ?? 0);

                smoothTimelines.Add(new SmoothSponsorshipTimeline()
                {
                    DateTimeRange = (start, end),
                    AdvertiserIdentifiers = advertiserExclusivities,
                    ClashExternalReferences = clashExclusivities,
                    SponsoredProducts = sponsoredItem.Products,
                    Applicability = sponsoredItem.Applicability ?? SponsorshipApplicability.AllCompetitors,
                    CalculationType = sponsoredItem.CalculationType
                });
            }
        }

        private List<(string advertiserIdentifier, int restrictionValue)> GetAdvertiserReferenceAndRestrictionValue(
            List<AdvertiserExclusivity> advertiserExclusivities,
            int restrictionValue) =>
            advertiserExclusivities
            ?.Select(a =>
            (a.AdvertiserIdentifier, a.RestrictionValue ?? restrictionValue))
            .ToList() ??
            new List<(string AdvertiserIdentifier, int restrictionValue)>();

        private List<(string clashExternalRef, int restrictionValue)> GetClashReferenceAndRestrictionValue(
            List<ClashExclusivity> clashExclusivities,
            int restrictionValue) =>
            clashExclusivities
            ?.Select(c =>
            (c.ClashExternalRef, c.RestrictionValue ?? restrictionValue))
            .ToList() ??
            new List<(string clashExternalRef, int restrictionValue)>();

        private IEnumerable<SponsoredItem> ExtractSponsoredItemsBySalesArea()
        {
            IList<SponsoredItem> sponsoredItems = new List<SponsoredItem>();

            foreach (var sponsoredItem in _sponsorship
                .SelectMany(s => s.SponsoredItems))
            {
                var clone = sponsoredItem.Clone();

                clone.SponsorshipItems = sponsoredItem.
                    SponsorshipItems
                    .Where(item =>
                        item.SalesAreas.Contains(_salesArea));

                sponsoredItems.Add(clone);
            }

            return sponsoredItems;
        }
    }
}
