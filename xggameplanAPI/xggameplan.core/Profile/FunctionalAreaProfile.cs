using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class FunctionalAreaProfile : AutoMapper.Profile
    {
        public FunctionalAreaProfile()
        {
            CreateMap<FunctionalArea, FunctionalAreaModel>();
        }
    }
}
