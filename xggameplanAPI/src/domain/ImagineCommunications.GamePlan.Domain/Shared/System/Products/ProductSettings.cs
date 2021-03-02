using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Products
{
    /// <summary>
    /// Product settings
    /// </summary>
    public class ProductSettings
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of enabled features
        /// </summary>
        public List<Feature> Features = new List<Feature>();

        /// <summary>
        /// Whether feature is enabled
        /// </summary>
        /// <param name="featureId"></param>
        /// <returns></returns>
        public bool IsFeatureEnabled(string featureId)
        {
            Feature feature = Features.Where(f => f.Id.ToUpper() == featureId.ToUpper()).FirstOrDefault();
            return (feature != null && feature.Enabled);
        }
    }
}
