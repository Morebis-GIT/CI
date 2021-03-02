using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class AnalysisGroupProfile : AutoMapper.Profile
    {
        private const string MissingMessage = "-MISSING-";

        public AnalysisGroupProfile()
        {
            CreateMap<(AnalysisGroup, AnalysisGroupFilterSearchModel, Dictionary<int, UserReducedModel>), AnalysisGroupModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Item1.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Item1.Name))
                .ForMember(x => x.DateCreated, opt => opt.MapFrom(src => src.Item1.DateCreated))
                .ForMember(x => x.DateModified, opt => opt.MapFrom(src => src.Item1.DateModified))
                .ForMember(x => x.CreatedBy, opt => opt.MapFrom(src => src.Item3.ContainsKey(src.Item1.CreatedBy) ? src.Item3[src.Item1.CreatedBy] : null))
                .ForMember(x => x.ModifiedBy, opt => opt.MapFrom(src => src.Item3.ContainsKey(src.Item1.ModifiedBy) ? src.Item3[src.Item1.ModifiedBy] : null))
                .ForMember(dest => dest.Filter, opt => opt.MapFrom(src => MapToAnalysisGroupFilterModel(src.Item1.Filter, src.Item2)));

            CreateMap<(List<AnalysisGroup>, AnalysisGroupFilterSearchModel, Dictionary<int, UserReducedModel>), List<AnalysisGroupModel>>()
                .ConvertUsing((src, desc, context) => src.Item1.Select(x => context.Mapper.Map<AnalysisGroupModel>((x, src.Item2, src.Item3))).ToList());

            CreateMap<CreateAnalysisGroupModel, AnalysisGroup>();
            CreateMap<CreateAnalysisGroupFilterModel, AnalysisGroupFilter>();
            CreateMap<AnalysisGroup, AnalysisGroupNameModel>();
        }

        private static AnalysisGroupFilterModel MapToAnalysisGroupFilterModel(AnalysisGroupFilter filter, AnalysisGroupFilterSearchModel displayValues) =>
            new AnalysisGroupFilterModel
            {
                Advertisers = ConstructFilterItem(filter.AdvertiserExternalIds, displayValues.Advertisers),
                Agencies = ConstructFilterItem(filter.AgencyExternalIds, displayValues.Agencies),
                MediaGroup = ConstructFilterItem(filter.AgencyGroupCodes, displayValues.AgencyGroups),
                BusinessTypes = filter.BusinessTypes.Select(x => new AnalysisGroupFilterItem<string>
                {
                    Key = x,
                    Label = displayValues.BusinessTypes.Contains(x) ? x : MissingMessage
                }).ToList(),
                Campaigns = ConstructFilterItem(filter.CampaignExternalIds, displayValues.Campaigns),
                ClashCodes = ConstructFilterItem(filter.ClashExternalRefs, displayValues.ClashCodes),
                Products = ConstructFilterItem(filter.ProductExternalIds, displayValues.Products),
                ReportingCategories = filter.ReportingCategories.Select(x => new AnalysisGroupFilterItem<string>
                {
                    Key = x,
                    Label = displayValues.ReportingCategories.Contains(x) ? x : MissingMessage
                }).ToList(),
                ProductAssigneeName = ConstructFilterItem(filter.SalesExecExternalIds, displayValues.SalesExecs)
            };

        private static List<AnalysisGroupFilterItem<TKey>> ConstructFilterItem<TKey>(IEnumerable<TKey> keysCollection, IReadOnlyDictionary<TKey, string> keyValueMappings) =>
            keysCollection.Select(key => new AnalysisGroupFilterItem<TKey>
            {
                Key = key,
                Label = keyValueMappings.ContainsKey(key) ? keyValueMappings[key] : MissingMessage
            }).ToList();
    }
}
