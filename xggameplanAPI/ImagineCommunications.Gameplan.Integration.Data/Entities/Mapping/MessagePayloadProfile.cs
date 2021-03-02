using AutoMapper;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class MessagePayloadProfile : Profile
    {
        public MessagePayloadProfile()
        {
            CreateMap<BusClient.Domain.Entities.MessagePayload, MessagePayload>().ReverseMap();
        }
    }
}
