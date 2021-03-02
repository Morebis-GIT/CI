using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class WeightingProfile : AutoMapper.Profile
    {
        public WeightingProfile()
        {
            CreateMap<Weighting, WeightingModel>();
            CreateMap<WeightingModel, Weighting>(); 
        }
    }
}
