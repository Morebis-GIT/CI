using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class BreakExclusionProfile : AutoMapper.Profile
    {
        public BreakExclusionProfile()
        {
            CreateMap<BreakExclusion, BreakExclusionModel>();
            CreateMap<BreakExclusionModel, BreakExclusion>();
        }
    }
}
