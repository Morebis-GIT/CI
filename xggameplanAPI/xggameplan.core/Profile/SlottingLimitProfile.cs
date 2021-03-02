using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class SlottingLimitProfile : AutoMapper.Profile
    {
        public SlottingLimitProfile()
        {
            CreateMap<SlottingLimit, SlottingLimitModel>();
            CreateMap<SlottingLimitModel, SlottingLimit>();
        }
    }
}
