using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ChannelProfile : AutoMapper.Profile
    {
        public ChannelProfile()
        {
            CreateMap<Channel, ChannelModel>().ForMember(cm => cm.Uid, expression => expression.MapFrom(c => c.Id));

            CreateMap<CreateChannelModel, Channel>();
        }
    }
}
