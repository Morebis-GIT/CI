using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ProgrammeRepetitionProfile: AutoMapper.Profile
    {
        public ProgrammeRepetitionProfile()
        {
            CreateMap<ProgrammeRepetition, ProgrammeRepetitionModel>();
            CreateMap<ProgrammeRepetitionModel, ProgrammeRepetition>();
        }
    }
}
