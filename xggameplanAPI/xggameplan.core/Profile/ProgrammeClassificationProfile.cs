using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ProgrammeClassificationProfile : AutoMapper.Profile
    {
        public ProgrammeClassificationProfile()
        {
            CreateMap<ProgrammeClassification, ProgrammeClassificationModel>();
            CreateMap<ProgrammeClassificationModel, ProgrammeClassification>();
        }
    }
}
