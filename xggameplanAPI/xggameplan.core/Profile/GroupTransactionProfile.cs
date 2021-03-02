using ImagineCommunications.BusClient.Domain.Entities;
using xggameplan.model.Internal.Landmark;

namespace xggameplan.core.Profile
{
    internal class GroupTransactionProfile : AutoMapper.Profile
    {
        public GroupTransactionProfile()
        {
            CreateMap<GroupTransactionInfo, GroupTransactionInfoModel>();
        }
    }
}
