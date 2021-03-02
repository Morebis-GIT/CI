using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.model.AutoGen.AgSponsorships;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class ProductProfile : AutoMapper.Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProduct, Product>();
            CreateMap<Product, ProductModel>()
                .ForMember(dest => dest.SalesExecutiveName,
                    opt => opt.MapFrom(src => src.SalesExecutive != null ? src.SalesExecutive.Name : null));
            CreateMap<Product, ProductNameModel>();
            CreateMap<Product, ProductAdvertiserModel>();
            CreateMap<string, AgProductCode>()
                .ForMember(d => d.Code, m => m.MapFrom(src => src));
            CreateMap<AgencyGroup, AgencyGroupModel>();
        }
    }
}
