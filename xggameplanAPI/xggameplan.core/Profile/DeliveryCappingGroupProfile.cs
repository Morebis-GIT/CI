using System;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using xggameplan.Model;
using xggameplan.Model.AutoGen;

namespace xggameplan.core.Profile
{
    internal class DeliveryCappingGroupProfile : AutoMapper.Profile
    {
        public DeliveryCappingGroupProfile()
        {
            CreateMap<DeliveryCappingGroup, DeliveryCappingGroupModel>().ReverseMap();
            CreateMap<DeliveryCappingGroup, AgDeliveryCappingGroup>()
                .ForMember(x => x.ApplyToPrice, opt => opt.MapFrom(src => Convert.ToInt32(src.ApplyToPrice)));
        }
    }
}
