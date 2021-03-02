using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using xggameplan.CSVImporter;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class FailureProfile : AutoMapper.Profile
    {
        public FailureProfile()
        {
            CreateMap<Failure, FailureModel>();
            CreateMap<FailureImportSummary, Failure>();
        }
    }
}
