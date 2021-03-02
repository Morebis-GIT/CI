using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ChannelProfile : Profile
    {
        public ChannelProfile()
        {
            CreateMap<Channel, Domain.Shared.Channels.Channel>().ReverseMap();
        }
    }
}
