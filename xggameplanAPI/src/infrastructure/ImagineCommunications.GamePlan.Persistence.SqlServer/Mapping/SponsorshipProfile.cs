using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using xggameplan.core.Extensions;
using SponsorshipEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.Sponsorship;
using SponsoredItemEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.SponsoredItem;
using SponsorshipAdvertiserExclusivityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.SponsorshipAdvertiserExclusivity;
using SponsorshipClashExclusivityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.SponsorshipClashExclusivity;
using SponsorshipItemEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.SponsorshipItem;
using SponsoredDayPartEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships.SponsoredDayPart;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SponsorshipProfile : Profile
    {
        public SponsorshipProfile()
        {
            CreateMap<Sponsorship, SponsorshipEntity>()
                .ReverseMap();

            CreateMap<AdvertiserExclusivity, SponsorshipAdvertiserExclusivityEntity>()
                .ReverseMap();

            CreateMap<ClashExclusivity, SponsorshipClashExclusivityEntity>()
                .ReverseMap();

            CreateMap<SponsoredItem, SponsoredItemEntity>()
                .ReverseMap();

            CreateMap<SponsorshipItem, SponsorshipItemEntity>()
                .ReverseMap();

            CreateMap<SponsoredDayPart, SponsoredDayPartEntity>()
                .ForMember(d => d.DaysOfWeek, opt => opt.MapFrom(s => AgConversions.ToDaysOfWeek(s.DaysOfWeek)))
                .ReverseMap()
                .ForMember(d => d.DaysOfWeek, opt => opt.MapFrom(s => s.DaysOfWeek.Select(x => x.ToString())));

            CreateMap<SponsorshipRestrictionLevel, Entities.SponsorshipRestrictionLevel>()
                .ConvertUsing(src => (Entities.SponsorshipRestrictionLevel) src);

            CreateMap<Entities.SponsorshipRestrictionLevel, SponsorshipRestrictionLevel>()
                .ConvertUsing(src => (SponsorshipRestrictionLevel) src);
            CreateMap<Entities.SponsorshipRestrictionLevel, SponsorshipRestrictionLevel>()
                .ConvertUsing(src => (SponsorshipRestrictionLevel) src);

            CreateMap<SponsorshipCalculationType, Entities.SponsorshipCalculationType>()
                .ConvertUsing(src => (Entities.SponsorshipCalculationType) src);
            CreateMap<Entities.SponsorshipCalculationType, SponsorshipCalculationType>()
                .ConvertUsing(src => (SponsorshipCalculationType) src);
            CreateMap<Entities.SponsorshipCalculationType, SponsorshipCalculationType>()
                .ConvertUsing(src => (SponsorshipCalculationType) src);

            CreateMap<SponsorshipRestrictionType, Entities.SponsorshipRestrictionType>()
                .ConvertUsing(src => (Entities.SponsorshipRestrictionType) src);
            CreateMap<Entities.SponsorshipRestrictionType?, SponsorshipRestrictionType?>()
                .ConvertUsing(src => (SponsorshipRestrictionType?) src);
            CreateMap<Entities.SponsorshipRestrictionType?, SponsorshipRestrictionType?>()
                .ConvertUsing(src => (SponsorshipRestrictionType?) src);

            CreateMap<SponsorshipApplicability, Entities.SponsorshipApplicability>()
                .ConvertUsing(src => (Entities.SponsorshipApplicability) src);
            CreateMap<Entities.SponsorshipApplicability?, SponsorshipApplicability?>()
                .ConvertUsing(src => (SponsorshipApplicability?) src);
            CreateMap<Entities.SponsorshipApplicability?, SponsorshipApplicability?>()
                .ConvertUsing(src => (SponsorshipApplicability?) src);
        }
    }
}
