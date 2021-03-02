using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.model.AutoGen.AgSponsorships;

namespace xggameplan.core.Profile
{
    internal class ClashExclusivityProfile : AutoMapper.Profile
    {
        public ClashExclusivityProfile()
        {
            CreateMap<ClashExclusivity, AgClashExclusivity>()
                 .ForMember(d => d.ClashCode, m => m.MapFrom(src => src.ClashExternalRef))
                 .ForMember(d => d.RestrictionType, m => m.MapFrom(src => src.RestrictionType.HasValue ? (int)src.RestrictionType.Value : -1))
                 .ForMember(d => d.RestrictionValue, m => m.MapFrom(src => src.RestrictionValue ?? -1));
        }
    }
}
