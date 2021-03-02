using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Data.Dto;
using GroupTransactionInfoDomain = ImagineCommunications.BusClient.Domain.Entities.GroupTransactionInfo;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class GroupTransactionInfoProfile : Profile
    {
        public GroupTransactionInfoProfile()
        {
            CreateMap<GroupTransactionInfoDomain, GroupTransactionInfo>().ReverseMap();
            CreateMap<GroupTransactionInfoDto, GroupTransactionInfoDomain>();
        }
    }
}
