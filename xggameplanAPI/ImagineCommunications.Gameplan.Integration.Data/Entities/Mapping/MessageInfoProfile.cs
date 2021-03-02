using AutoMapper;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class MessageInfoProfile : Profile
    {
        public MessageInfoProfile()
        {
            CreateMap<BusClient.Domain.Entities.MessageInfo, MessageInfo>().ReverseMap();
        }
    }
}
