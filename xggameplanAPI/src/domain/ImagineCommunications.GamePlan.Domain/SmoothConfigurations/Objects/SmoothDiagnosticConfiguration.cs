using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Smooth diagnosic configuration
    /// </summary>
    public class SmoothDiagnosticConfiguration
    {
        /// <summary>
        /// Sales areas to filter for spots
        /// </summary>
        public List<string> SpotSalesAreas { get; set; }

        /// <summary>
        /// Demographics to filter for spots
        /// </summary>
        public List<string> SpotDemographics { get; set; }

        /// <summary>
        /// Spots to filter
        /// </summary>
        public List<string> SpotExternalRefs { get; set; }

        /// <summary>
        /// Campaigns to filter for spots
        /// </summary>
        public List<string> SpotExternalCampaignRefs { get; set; }

        /// <summary>
        /// Multipart spot values to filter for spots
        /// </summary>
        public List<string> SpotMultipartSpots { get; set; }

        /// <summary>
        /// Min start time to filter for spots
        /// </summary>
        public DateTime? SpotMinStartTime { get; set; }

        /// <summary>
        /// Max start time to filter for spots
        /// </summary>
        public DateTime? SpotMaxStartTime { get; set; }

        /// <summary>
        /// Min preempt level to filter for spots
        /// </summary>
        public int? SpotMinPreemptLevel { get; set; }

        /// <summary>
        /// Max preempt level to filter for spots
        /// </summary>
        public int? SpotMaxPreemptLevel { get; set; }
    }
}
