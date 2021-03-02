using AutoMapper;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities.Mapping
{
    public class MessageEntityTypeProfile : Profile
    {
        public MessageEntityTypeProfile() 
        {
            CreateMap<BusClient.Domain.Entities.MessageEntityType, MessageEntityType>()
                .ReverseMap();
        }
    }
}
