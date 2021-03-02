using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using AgencyGroup = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AgencyGroup;
using Product = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, Domain.Shared.Products.Objects.Product>()
                .ConstructUsing((p, context) =>
                {
                    var product = new Domain.Shared.Products.Objects.Product();

                    var productAdvertiser = p.ProductAdvertisers?.FirstOrDefault();
                    if (!(productAdvertiser is null))
                    {
                        product.AdvertiserIdentifier = productAdvertiser.Advertiser?.ExternalIdentifier;
                        product.AdvertiserName = productAdvertiser.Advertiser?.Name;
                        product.AdvertiserLinkStartDate = productAdvertiser.StartDate;
                        product.AdvertiserLinkEndDate = productAdvertiser.EndDate;
                    }

                    var productAgency = p.ProductAgencies?.FirstOrDefault();
                    if (!(productAgency is null))
                    {
                        product.AgencyIdentifier = productAgency.Agency?.ExternalIdentifier;
                        product.AgencyName = productAgency.Agency?.Name;
                        product.AgencyStartDate = productAgency.StartDate;
                        product.AgencyLinkEndDate = productAgency.EndDate;
                        if (!(productAgency.AgencyGroup is null))
                        {
                            product.AgencyGroup = new Domain.Shared.Products.Objects.AgencyGroup
                            {
                                ShortName = productAgency.AgencyGroup.ShortName,
                                Code = productAgency.AgencyGroup.Code
                            };
                        }
                    }

                    var productPerson = p.ProductPersons?.FirstOrDefault();
                    if (!(productPerson is null))
                    {
                        product.SalesExecutive = new SalesExecutive
                        {
                            Identifier = productPerson.Person?.ExternalIdentifier ?? 0,
                            Name = productPerson.Person?.Name,
                            StartDate = productPerson.StartDate,
                            EndDate = productPerson.EndDate
                        };
                    }

                    return product;
                })
                .ReverseMap();

            CreateMap<ProductDto, Domain.Shared.Products.Objects.Product>()
                .ForMember(dest => dest.AdvertiserLinkStartDate, opt => opt.MapFrom(src => src.AdvertiserStartDate))
                .ForMember(dest => dest.AdvertiserLinkEndDate, opt => opt.MapFrom(src => src.AdvertiserEndDate))
                .ForMember(dest => dest.AgencyLinkEndDate, opt => opt.MapFrom(src => src.AgencyEndDate))
                .ForMember(dest => dest.AgencyGroup,
                    opt => opt.MapFrom(src =>
                        src.AgencyGroupId != null
                            ? new Domain.Shared.Products.Objects.AgencyGroup
                            {
                                Code = src.AgencyGroupCode, ShortName = src.AgencyGroupShortName
                            }
                            : null))
                .ForMember(dest => dest.SalesExecutive,
                    opt => opt.MapFrom(src =>
                        src.PersonId != null
                            ? new SalesExecutive
                            {
                                Identifier = src.PersonIdentifier.Value,
                                Name = src.PersonName,
                                StartDate = src.PersonStartDate.Value,
                                EndDate = src.PersonEndDate.Value
                            }
                            : null))
                ;
            CreateMap<ProductDto, ProductNameModel>();
            CreateMap<Advertiser, Domain.Shared.Products.Objects.ProductAdvertiserModel>()
                .ForMember(dest => dest.AdvertiserName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.AdvertiserIdentifier, opt => opt.MapFrom(src => src.ExternalIdentifier));
            CreateMap<Product, Domain.Shared.Products.Objects.ProductNameModel>().ReverseMap();
            CreateMap<Person, Domain.Shared.Products.Objects.SalesExecutive>()
                .ForMember(dest => dest.Identifier, opt => opt.MapFrom(src => src.ExternalIdentifier))
                .ReverseMap()
                .ForMember(dest => dest.ExternalIdentifier, opt => opt.MapFrom(src => src.Identifier));
            CreateMap<Person, Domain.Shared.Products.Objects.SalesExecutiveModel>()
                .ForMember(dest => dest.Identifier, opt => opt.MapFrom(src => src.ExternalIdentifier));

            CreateMap<AgencyGroup, Domain.Shared.Products.Objects.AgencyGroup>().ReverseMap();

            CreateMap<Agency, Domain.Shared.Products.Objects.ProductAgencyModel>()
                .ForMember(dest => dest.AgencyIdentifier, opt => opt.MapFrom(src => src.ExternalIdentifier))
                .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
