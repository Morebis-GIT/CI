using System;
using System.Collections.Generic;

namespace xggameplan.model.External
{
    public class RunTypeModel
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
        /// Determine if the run type is hidden
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Value for the default KPI
        /// </summary>
        public string DefaultKPI { get; set; }

        /// <summary>
        /// Run Type Modified Date
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// List of Analysis Groups associated to the Run Type
        /// </summary>
        public List<RunTypeAnalysisGroupModel> RunTypeAnalysisGroups { get; set; } = new List<RunTypeAnalysisGroupModel>();

        public RunLandmarkScheduleSettingsModel RunLandmarkScheduleSettings { get; set; }
    }
}
