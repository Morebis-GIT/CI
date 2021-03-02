using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using xggameplan.Model;

namespace xggameplan.Profile
{

    internal class UniverseProfile : AutoMapper.Profile
    {
        public UniverseProfile()
        {
            CreateMap<UniverseModel, Universe>();
            CreateMap<Universe, UniverseModel>();
            CreateMap<CreateUniverse, Universe>();
        }
    }
}
