using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using xggameplan.model.External;

namespace xggameplan.Profile
{
    internal class ClashExceptionProfile : AutoMapper.Profile
    {
        public ClashExceptionProfile()
        {
            CreateMap<ClashException, ClashExceptionModel>();
            CreateMap<CreateClashException, ClashException>();
        }
    }
}
