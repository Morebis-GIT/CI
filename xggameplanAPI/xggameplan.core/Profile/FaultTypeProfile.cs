using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class FaultTypeProfile : AutoMapper.Profile
    {
        public FaultTypeProfile()
        {
            CreateMap<FaultType, FaultTypeModel>();
        }
    }
}
