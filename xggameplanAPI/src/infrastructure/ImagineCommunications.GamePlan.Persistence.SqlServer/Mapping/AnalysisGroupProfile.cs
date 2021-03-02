using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AnalysisGroups;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AnalysisGroupProfile : Profile
    {
        public AnalysisGroupProfile()
        {
            CreateMap<AnalysisGroup, Domain.AnalysisGroups.Objects.AnalysisGroup>()
                .ForPath(x => x.Filter.AdvertiserExternalIds, opt => opt.MapFrom(src => src.FilterAdvertiserExternalIds))
                .ForPath(x => x.Filter.AgencyExternalIds, opt => opt.MapFrom(src => src.FilterAgencyExternalIds))
                .ForPath(x => x.Filter.AgencyGroupCodes, opt => opt.MapFrom(src => src.FilterAgencyGroupCodes))
                .ForPath(x => x.Filter.BusinessTypes, opt => opt.MapFrom(src => src.FilterBusinessTypes))
                .ForPath(x => x.Filter.CampaignExternalIds, opt => opt.MapFrom(src => src.FilterCampaignExternalIds))
                .ForPath(x => x.Filter.ClashExternalRefs, opt => opt.MapFrom(src => src.FilterClashExternalRefs))
                .ForPath(x => x.Filter.ProductExternalIds, opt => opt.MapFrom(src => src.FilterProductExternalIds))
                .ForPath(x => x.Filter.ReportingCategories, opt => opt.MapFrom(src => src.FilterReportingCategories))
                .ForPath(x => x.Filter.SalesExecExternalIds, opt => opt.MapFrom(src => src.FilterSalesExecExternalIds))
                .ReverseMap();

            CreateMap<AnalysisGroup, Domain.AnalysisGroups.Objects.AnalysisGroupNameModel>();
        }
    }
}
