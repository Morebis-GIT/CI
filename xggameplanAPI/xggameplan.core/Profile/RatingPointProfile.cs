using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.core.Profile
{
    internal class RatingPointProfile : AutoMapper.Profile
    {
        public RatingPointProfile()
        {
            CreateMap<RatingPoint, RatingPointModel>().ReverseMap();
        }
    }
}
