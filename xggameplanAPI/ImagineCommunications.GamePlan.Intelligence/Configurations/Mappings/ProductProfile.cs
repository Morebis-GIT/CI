using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Product;
using AdvertiserEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Advertiser;
using AgencyEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Agency;
using AgencyGroupEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AgencyGroup;
using PersonEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Person;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.GamePlan.Intelligence.Configurations.Mappings
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<IProductCreatedOrUpdated, ProductEntity>();
            CreateMap<Advertiser, AdvertiserEntity>()
                .ForMember(dest => dest.ExternalIdentifier, opt => opt.MapFrom(src => src.AdvertiserIdentifier));
            CreateMap<Agency, AgencyEntity>()
                .ForMember(dest => dest.ExternalIdentifier, opt => opt.MapFrom(src => src.AgencyIdentifier));
            CreateMap<Agency, AgencyGroupEntity>()
                .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.AgencyGroupShortName))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.AgencyGroupCode));
            CreateMap<Person, PersonEntity>()
                .ForMember(dest => dest.ExternalIdentifier, opt => opt.MapFrom(src => src.PersonIdentifier));
        }
    }
}
