using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using NodaTime;
using Raven.Client.UniqueConstraints;

namespace ImagineCommunications.GamePlan.Domain.Shared.SalesAreas
{
    public class SalesArea : ICloneable
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        [UniqueConstraint]
        public int CustomId { get; set; }

        /// <summary>
        /// Raven Document unique Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Sales Area Name
        /// </summary>
        [UniqueConstraint]
        public string Name { get; set; }

        /// <summary>
        /// Sales Area Short Name
        /// </summary>
        [UniqueConstraint]
        public string ShortName { get; set; }

        /// <summary>
        /// Currency Code
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Base Demographic One
        /// </summary>
        public string BaseDemographic1 { get; set; }

        /// <summary>
        /// Base Demographic Two
        /// </summary>
        public string BaseDemographic2 { get; set; }

        /// <summary>
        /// Universe
        /// </summary>
        public int UniverseId { get; set; }

        /// <summary>
        /// Target Area within which the Sales Area is defined
        /// </summary>
        public string TargetAreaName { get; set; }

        /// <summary>
        /// Channel Group - list of channels
        /// </summary>
        public List<string> ChannelGroup = new List<string>();

        /// <summary>
        /// Public holidays
        /// </summary>
        public List<DateRange> PublicHolidays = new List<DateRange>();

        /// <summary>
        /// School holidays
        /// </summary>
        public List<DateRange> SchoolHolidays = new List<DateRange>();

        /// <summary>
        /// Start Offset from Midnight
        /// </summary>
        public Duration StartOffset { get; set; }

        /// <summary>
        /// Day Duration
        /// </summary>
        public Duration DayDuration { get; set; }

        public static SalesArea MapFrom(
            Guid id,
            string name,
            string shortName,
            string currencyCode,
            string baseDemographic1,
            string baseDemographic2,
            List<string> channelGroup,
            Duration startDay,
            Duration dayDuration)
        {
            Validate(id,
                name,
                shortName,
                currencyCode,
                baseDemographic1,
                baseDemographic2,
                channelGroup,
                dayDuration);

            return new SalesArea()
            {
                Id = id,
                Name = name,
                ShortName = shortName,
                CurrencyCode = currencyCode,
                BaseDemographic1 = baseDemographic1,
                BaseDemographic2 = baseDemographic2,
                ChannelGroup = channelGroup,
                StartOffset = startDay,
                DayDuration = dayDuration
            };
        }

        /// <summary>
        /// Updates SalesAreaValues except id
        /// </summary>
        public void Update(
            Guid id,
            string name,
            string shortName,
            string currencyCode,
            string baseDemographic1,
            string baseDemographic2,
            List<string> channelGroup,
            Duration startDay,
            Duration dayDuration
            )
        {
            Validate(id,
                name,
                shortName,
                currencyCode,
                baseDemographic1,
                baseDemographic2,
                channelGroup,
                dayDuration);

            Name = name;
            ShortName = shortName;
            CurrencyCode = currencyCode;
            BaseDemographic1 = baseDemographic1;
            BaseDemographic2 = baseDemographic2;
            ChannelGroup = channelGroup;
            StartOffset = startDay;
            DayDuration = dayDuration;
        }

        private static void Validate(
            Guid id,
            string name,
            string shortName,
            string currencyCode,
            string baseDemographic1,
            string baseDemographic2,
            List<string> channelGroup,
            Duration dayDuration)

        {
            if (id == default)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(shortName))
            {
                throw new ArgumentNullException(nameof(shortName));
            }

            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                throw new ArgumentNullException(nameof(currencyCode));
            }

            if (string.IsNullOrWhiteSpace(baseDemographic1))
            {
                throw new ArgumentNullException(nameof(baseDemographic1));
            }

            if (string.IsNullOrWhiteSpace(baseDemographic2))
            {
                throw new ArgumentNullException(nameof(baseDemographic2));
            }

            // In future, we can also check if the channels in the list exists in channels repository
            if (channelGroup.Count == 0)
            {
                throw new ArgumentNullException(nameof(channelGroup));
            }

            if (dayDuration.BclCompatibleTicks == 0)
            {
                throw new ArgumentNullException(nameof(dayDuration), "Day Duration must be greater than zero");
            }
        }

        /// <summary>
        /// Indexes list by Name
        /// </summary>
        /// <param name="salesAreas"></param>
        /// <returns></returns>
        public static Dictionary<string, SalesArea> IndexListByName(IEnumerable<SalesArea> salesAreas)
        {
            var salesAreasByName = new Dictionary<string, SalesArea>();
            foreach (var salesArea in salesAreas)
            {
                if (!salesAreasByName.ContainsKey(salesArea.Name))
                {
                    salesAreasByName.Add(salesArea.Name, salesArea);
                }
            }

            return salesAreasByName;
        }

        /// <summary>
        /// Indexes list by Name
        /// </summary>
        /// <param name="salesAreas"></param>
        /// <returns></returns>
        public static Dictionary<int, SalesArea> IndexListByCustomId(IEnumerable<SalesArea> salesAreas)
        {
            var salesAreasIndexed = new Dictionary<int, SalesArea>();

            foreach (var salesArea in salesAreas)
            {
                if (!salesAreasIndexed.ContainsKey(salesArea.CustomId))
                {
                    salesAreasIndexed.Add(salesArea.CustomId, salesArea);
                }
            }

            return salesAreasIndexed;
        }

        public object Clone() => MemberwiseClone();
    }
}
