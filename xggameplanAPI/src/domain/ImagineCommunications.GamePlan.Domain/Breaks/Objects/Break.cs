using System;
using System.Collections.Generic;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Breaks.Objects
{
    public class Break : IBreakAvailability, ICloneable
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Unique Id
        /// </summary>
        public int CustomId { get; set; }

        public DateTime ScheduledDate { get; set; }
        public DateTime? BroadcastDate { get; set; }
        public int? ClockHour { get; set; }

        public string SalesArea { get; set; }
        public string BreakType { get; set; }
        public Duration Duration { get; set; }
        public Duration ReserveDuration { get; set; }
        /// <summary>
        /// Availability is break duration minus booked spots.
        /// </summary>
        public Duration Avail { get; set; }

        /// <summary>
        /// <para>
        /// Availability passed to Optimizer. Must be the <see cref="Avail"/>
        /// minus unplaced Smooth spots so that Optimizer does not overbook.
        /// </para>
        /// </summary>
        /// <example>
        /// E.g. if B1.Avail is 15 seconds, B2.Avail is 15 seconds, B3.Avail is
        /// zero seconds and the unplaced programme spots are 20 seconds then
        /// B1.OptimizerAvail and B2.OptimizerAvail will be reduced by 10
        /// seconds each.
        /// </example>
        public Duration OptimizerAvail { get; set; }

        public bool Optimize { get; set; }
        public string ExternalBreakRef { get; set; }
        public string Description { get; set; }

        public string ExternalProgRef { get; set; }
        public BreakPosition PositionInProg { get; set; }
        public double BreakPrice { get; set; }
        public double FloorRate { get; set; }

        /// <summary>
        /// Solus = All availability for a break is occupied by one spot
        /// </summary>
        public bool Solus { get; set; }
        public string PremiumCategory { get; set; }
        public bool AllowSplit { get; set; }
        public bool NationalRegionalSplit { get; set; }
        public bool ExcludePackages { get; set; }
        public bool BonusAllowed { get; set; }
        public bool LongForm { get; set; }

        [Obsolete("Is this used?")]
        public List<Guid> SpotReferencesList { get; set; }

        public List<BreakEfficiency> BreakEfficiencyList { get; set; }

        /// <summary>
        /// Indexes list by ExternalId
        /// </summary>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public static Dictionary<string, Break> IndexListByExternalId(IEnumerable<Break> breaks)
        {
            var breaksByExternalRef = new Dictionary<string, Break>();

            foreach (var oneBreak in breaks)
            {
                if (!breaksByExternalRef.ContainsKey(oneBreak.ExternalBreakRef))
                {
                    breaksByExternalRef.Add(oneBreak.ExternalBreakRef, oneBreak);
                }
            }

            return breaksByExternalRef;
        }

        public object Clone() => MemberwiseClone();
    }
}
