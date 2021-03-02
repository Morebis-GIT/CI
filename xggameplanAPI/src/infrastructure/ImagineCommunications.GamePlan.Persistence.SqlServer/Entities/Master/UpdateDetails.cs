using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class UpdateDetails: IUniqueIdentifierPrimaryKey
    {
        /// <summary>
        /// Update ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Update name
        /// </summary>
        public string Name { get; set; }

        public int TenantId { get; set; }

        /// <summary>
        /// Time applied
        /// </summary>
        public DateTime TimeApplied { get; set; }
    }
}
