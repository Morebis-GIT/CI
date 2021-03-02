using System;
using System.Collections.Generic;
using NodaTime;

namespace xggameplan.Model
{
    public class CreateSalesAreaModel
    {
        /// <summary>
        /// Sales Area Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sales Area Short Name
        /// </summary>
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
        /// Base Demographic One
        /// </summary>
        public string BaseDemographic2 { get; set; }

        /// <summary>
        /// Channel Group - list of channels
        /// </summary>
        public List<String> ChannelGroup { get; set; }

        /// <summary>
        /// Start OFfset from Midnight
        /// </summary>
        public Duration StartOffset { get; set; }

        /// <summary>
        /// Day Duration
        /// </summary>
        public Duration DayDuration { get; set; }
    }
}
