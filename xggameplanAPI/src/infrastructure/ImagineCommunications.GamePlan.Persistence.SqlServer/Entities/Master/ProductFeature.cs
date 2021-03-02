using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class ProductFeature : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int ProductSettingsId { get; set; }

        /// <summary>
        /// Whether feature is enabled
        /// </summary>
        public bool Enabled { get; set; }

        public string Settings { get; set; }
 
    }
}
