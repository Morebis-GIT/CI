using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    public class SponsorshipProfile : AutoMapper.Profile
    {
        public SponsorshipProfile()
        {
            //GET mappings
            CreateMap<Sponsorship, SponsorshipModel>();
            CreateMap<SponsoredItem, SponsoredItemModel>();
            CreateMap<SponsorshipItem, SponsorshipItemModel>();

            CreateMap<AdvertiserExclusivity, AdvertiserExclusivityModel>();
            CreateMap<ClashExclusivity, ClashExclusivityModel>();
            CreateMap<SponsoredDayPart, CreateSponsoredDayPartModel>()
                .ReverseMap();

            //API POST mappings
            CreateMap<CreateSponsorshipModel, Sponsorship>();
            CreateMap<CreateSponsoredItemModel, SponsoredItem>();

            CreateMap<CreateSponsorshipItemModel, SponsorshipItem>();

            CreateMap<CreateAdvertiserExclusivityModel, AdvertiserExclusivity>();
            CreateMap<CreateClashExclusivityModel, ClashExclusivity>();
        }
    }
}
