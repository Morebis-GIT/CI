using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class GeneralProfile : AutoMapper.Profile
    {
        public GeneralProfile()
        {
            CreateMap<General, GeneralModel>().ReverseMap();
        }
    }
}
