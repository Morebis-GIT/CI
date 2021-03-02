using AutoMapper;
using ImagineCommunications.Gameplan.Synchronization.Objects;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer.Mappings
{
    public class SynchronizationProfile : Profile
    {
        public SynchronizationProfile()
        {
            CreateMap<SynchronizationObject, Entities.SynchronizationObject>().ReverseMap();
            CreateMap<SynchronizationObjectOwner, Entities.SynchronizationObjectOwner>().ReverseMap();
        }
    }
}
