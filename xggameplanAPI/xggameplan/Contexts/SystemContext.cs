using ImagineCommunications.GamePlan.Domain.Shared.System.Products;

namespace xggameplan.Contexts
{
    /// <summary>
    /// System context.
    /// </summary>
    public class SystemContext
    {
        public SystemContext(ProductSettings productSettings)
        {
            ProductSettings = productSettings;
        }

        /// <summary>
        /// Product settings
        /// </summary>
        public ProductSettings ProductSettings { get; }
    }
}
