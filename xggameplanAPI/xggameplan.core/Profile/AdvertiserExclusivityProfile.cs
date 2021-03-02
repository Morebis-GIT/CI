using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.model.AutoGen.AgSponsorships;

namespace xggameplan.core.Profile
{
    internal class AdvertiserExclusivityProfile : AutoMapper.Profile
    {
        public AdvertiserExclusivityProfile()
        {
            CreateMap<AdvertiserExclusivity, AgAdvertiserExclusivity>()
                 .ForMember(d => d.Advertisercode, m => m.MapFrom(src => src.AdvertiserIdentifier))
                 .ForMember(d => d.RestrictionType, m => m.MapFrom(src => src.RestrictionType.HasValue ? (int)src.RestrictionType.Value : -1))
                 .ForMember(d => d.RestrictionValue, m => m.MapFrom(src => src.RestrictionValue.HasValue ? src.RestrictionValue.Value : -1));
        }
    }
}
