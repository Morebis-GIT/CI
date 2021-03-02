using System;

namespace ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail
{
    /// <summary>
    /// Update details
    /// </summary>
    public class UpdateDetails
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
