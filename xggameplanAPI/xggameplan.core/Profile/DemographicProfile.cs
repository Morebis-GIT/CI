using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class DemographicProfile : AutoMapper.Profile
    {
        public DemographicProfile()
        {            
            CreateMap<Demographic, DemographicModel>();
            CreateMap<DemographicModel, Demographic>();
            CreateMap<CreateDemographicModel, Demographic>();
        }
    }
}
