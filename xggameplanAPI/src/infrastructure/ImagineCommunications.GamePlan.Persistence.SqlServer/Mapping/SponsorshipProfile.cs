using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;
using SponsorshipEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Sponsorships;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class SponsorshipProfile : Profile
    {
        public SponsorshipProfile()
        {
            _ = CreateMap<Sponsorship, SponsorshipEntities.Sponsorship>()
                 .ReverseMap();

            _ = CreateMap<SponsorshipItem, SponsorshipEntities.SponsorshipItem>()
                .ReverseMap();
            _ = CreateMap<AdvertiserExclusivity, SponsorshipEntities.SponsorshipAdvertiserExclusivity>()
                .ReverseMap();

            _ = CreateMap<ClashExclusivity, SponsorshipEntities.SponsorshipClashExclusivity>()
                 .ReverseMap();

            _ = CreateMap<SponsoredItem, SponsorshipEntities.SponsoredItem>()
                 .ReverseMap();

            CreateMap<string, SponsorshipEntities.SponsorshipItemSalesArea>()
                .ForMember(dest => dest.SalesAreaId,
                    opts => opts.FromEntityCache(opt => opt.Entity<SalesArea>(x => x.Id)))
                .ReverseMap()
                .FromEntityCache(x => x.SalesAreaId, opt => opt.Entity<SalesArea>(x => x.Name));

            _ = CreateMap<SponsoredDayPart, SponsorshipEntities.SponsoredDayPart>()
                .ForMember(d => d.DaysOfWeek, opt => opt.MapFrom(s => AgConversions.ToDaysOfWeek(s.DaysOfWeek)))
                .ReverseMap()
                .ForMember(d => d.DaysOfWeek, opt => opt.MapFrom(s => s.DaysOfWeek.Select(x => x.ToString())));

            CreateMap<SponsorshipRestrictionLevel, Entities.SponsorshipRestrictionLevel>()
                .ConvertUsing(src => (Entities.SponsorshipRestrictionLevel)src);

            CreateMap<Entities.SponsorshipRestrictionLevel, SponsorshipRestrictionLevel>()
                .ConvertUsing(src => (SponsorshipRestrictionLevel)src);
            CreateMap<Entities.SponsorshipRestrictionLevel, SponsorshipRestrictionLevel>()
                .ConvertUsing(src => (SponsorshipRestrictionLevel)src);

            CreateMap<SponsorshipCalculationType, Entities.SponsorshipCalculationType>()
                .ConvertUsing(src => (Entities.SponsorshipCalculationType)src);
            CreateMap<Entities.SponsorshipCalculationType, SponsorshipCalculationType>()
                .ConvertUsing(src => (SponsorshipCalculationType)src);
            CreateMap<Entities.SponsorshipCalculationType, SponsorshipCalculationType>()
                .ConvertUsing(src => (SponsorshipCalculationType)src);

            CreateMap<SponsorshipRestrictionType, Entities.SponsorshipRestrictionType>()
                .ConvertUsing(src => (Entities.SponsorshipRestrictionType)src);
            CreateMap<Entities.SponsorshipRestrictionType?, SponsorshipRestrictionType?>()
                .ConvertUsing(src => (SponsorshipRestrictionType?)src);
            CreateMap<Entities.SponsorshipRestrictionType?, SponsorshipRestrictionType?>()
                .ConvertUsing(src => (SponsorshipRestrictionType?)src);

            CreateMap<SponsorshipApplicability, Entities.SponsorshipApplicability>()
                .ConvertUsing(src => (Entities.SponsorshipApplicability)src);
            CreateMap<Entities.SponsorshipApplicability?, SponsorshipApplicability?>()
                .ConvertUsing(src => (SponsorshipApplicability?)src);
            CreateMap<Entities.SponsorshipApplicability?, SponsorshipApplicability?>()
                .ConvertUsing(src => (SponsorshipApplicability?)src);
        }
    }
}
