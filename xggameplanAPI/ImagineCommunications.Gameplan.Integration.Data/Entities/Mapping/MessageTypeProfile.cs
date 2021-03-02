using AutoMapper;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class MessageTypeProfile : Profile
    {
        public MessageTypeProfile()
        {
            CreateMap<BusClient.Domain.Entities.MessageType, MessageType>()
                .ReverseMap();
        }
    }
}
