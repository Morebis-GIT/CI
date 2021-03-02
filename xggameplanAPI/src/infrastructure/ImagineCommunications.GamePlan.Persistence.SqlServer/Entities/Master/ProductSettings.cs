using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class ProductSettings : IIdentityPrimaryKey
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Product description
        /// </summary>
        public string Description { get; set; }

        public List<ProductFeature> Features { get; set; }
    }
}
