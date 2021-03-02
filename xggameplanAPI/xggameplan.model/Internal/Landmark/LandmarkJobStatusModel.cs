using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.Model
{
    public class LandmarkJobStatusModel
    {
        public ExternalScenarioStatus JobStatus { get; set; }
        public string ErrorMessage { get; set; }

        public List<LandmarkOutputFileModel> OutputFiles { get; set; }
    }
}
