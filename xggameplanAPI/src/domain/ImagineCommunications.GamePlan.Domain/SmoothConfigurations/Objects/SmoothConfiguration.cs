using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// Smooth configuration. This is the raw document that is stored in Raven and it is accessed
    /// at runtime via the SmoothConfigurationReader class.
    /// </summary>
    public class SmoothConfiguration
    {
        public static int DefaultId => 1;

        public int Id { get; set; }

        public string Version { get; set; }

        public bool RestrictionCheckEnabled { get; set; }

        public bool ClashExceptionCheckEnabled { get; set; }

        /// <summary>
        /// ExternalCampaignRefs to exclude from Smooth
        /// </summary>
        public List<string> ExternalCampaignRefsToExclude { get; set; }

        public bool RecommendationsForExcludedCampaigns { get; set; }

        public bool SmoothFailuresForExcludedCampaigns { get; set; }

        /// <summary>
        /// Smooth Passes
        /// </summary>
        public List<SmoothPass> Passes { get; set; }

        /// <summary>
        /// Iterations
        /// </summary>
        public List<SmoothPassIterationRecord> IterationRecords { get; set; }

        /// <summary>
        /// Best break factor groups
        /// </summary>
        public List<BestBreakFactorGroupRecord> BestBreakFactorGroupRecords { get; set; }

        /// <summary>
        /// Diagnostic configuration
        /// </summary>
        public SmoothDiagnosticConfiguration DiagnosticConfiguration { get; set; }
    }
}
