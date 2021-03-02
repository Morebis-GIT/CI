using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Settings;
using Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class ProductSettingsProfile : Profile
    {
        public ProductSettingsProfile()
        {
            CreateMap<ProductSettings, Entities.Master.ProductSettings>().
                ForMember(m => m.Features, opt =>
                {
                    opt.MapFrom(src => src.Features);
                })
                .ReverseMap();
                

            CreateMap<Feature, Entities.Master.ProductFeature>()
                .ForMember(m => m.Id, opt => opt.Ignore())
                .ForMember(m => m.Name, opt => opt.MapFrom(src => src.Id))
                .ForMember(m => m.Settings, opt =>
                {
                    opt.PreCondition(src => src.Settings != null);
                    opt.MapFrom(src => JsonConvert.SerializeObject(src.Settings));
                })
                .ReverseMap()
                .ForMember(m => m.Id, opt => opt.MapFrom(src => src.Name))
                .ForMember(x => x.Settings, opt =>
                 {
                     opt.PreCondition(src => string.IsNullOrWhiteSpace(src.Settings));
                     opt.MapFrom(src => JsonConvert.DeserializeObject<Dictionary<string, object>>(src.Settings));
                 });
        }
    }
}
