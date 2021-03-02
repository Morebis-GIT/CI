using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SmoothSponsorshipRestrictionLimits
    {
        /// <summary>
        /// Availability of all the products using for sponsorships
        /// </summary>
        /// <list>
        /// <listheader>
        /// <term>Key</term>
        /// <description>Product External Reference</description>
        /// </listheader>
        /// <item>
        /// <term>Value</term>
        /// <description>Availability (amount left in seconds or count)</description>
        /// </item>
        /// </list>
        public Dictionary<ProductExternalReference, double> AvailabilitiesForCompetitors { get; set; }
        = new Dictionary<ProductExternalReference, double>();
    }
}
