using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects
{
    /// <summary>
    /// AutoBook instance type
    /// </summary>
    public class AutoBookInstanceConfiguration
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Cloud provider (E.g. AWS)
        /// </summary>
        public CloudProviders CloudProvider { get; set; }

        /// <summary>
        /// Criteria for identifying best AutoBook
        /// </summary>
        public List<AutoBookInstanceConfigurationCriteria> CriteriaList = new List<AutoBookInstanceConfigurationCriteria>();

        /// <summary>
        /// Whether AutoBook type can execute run of this size
        /// </summary>
        /// <param name="days"></param>
        /// <param name="campaigns"></param>
        /// <returns></returns>
        public bool CanExecuteRun(int days, int salesAreas, int campaigns, int demographics, int breaks)
        {
            return CriteriaList != null && CriteriaList.Where(c => c.MeetsCriteria(days, salesAreas, campaigns, demographics, breaks)).Any();
        }

        /// <summary>
        /// Instance type (E.g. t2.small, t2.medium, t2.large etc)
        /// </summary>
        public string InstanceType { get; set; }

        /// <summary>
        /// Storage size (GB)
        /// </summary>
        public int StorageSizeGb { get; set; }

        /// <summary>
        /// Cost of instance, used for identifying the cheapest.
        /// </summary>
        public double Cost { get; set; }
    }
}
