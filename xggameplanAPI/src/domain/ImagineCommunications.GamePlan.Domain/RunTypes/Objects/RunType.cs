using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.RunTypes.Objects
{
    public class RunType
    {
        /// <summary>
        /// Run Type Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Run Type Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Run Type Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Run Type Hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Run Type Modified Date
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Value for the default KPI
        /// </summary>
        public string DefaultKPI { get; set; }

        /// <summary>
        /// List of Analysis Groups assigned to Run Type
        /// </summary>
        public List<RunTypeAnalysisGroup> RunTypeAnalysisGroups { get; set; } = new List<RunTypeAnalysisGroup>();

        public RunLandmarkScheduleSettings RunLandmarkScheduleSettings { get; set; }
    }
}
