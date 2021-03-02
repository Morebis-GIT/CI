using System.Collections.Generic;
using xggameplan.model.External;

namespace xggameplan.Model
{
    public class CreateRunTypeModel
    {
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
        /// Value for the default KPI
        /// </summary>
        public string DefaultKPI { get; set; }

        /// <summary>
        /// List of Analysis Groups assigned to Run Type
        /// </summary>
        public List<CreateRunTypeAnalysisGroupModel> RunTypeAnalysisGroups { get; set; }

        public CreateRunLandmarkScheduleSettingsModel RunLandmarkScheduleSettings { get; set; }
    }
}
