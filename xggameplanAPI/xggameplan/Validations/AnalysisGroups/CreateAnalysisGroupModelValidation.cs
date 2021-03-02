using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Validations.AnalysisGroups
{
    public class CreateAnalysisGroupModelValidation : AbstractValidator<CreateAnalysisGroupModel>
    {
        public CreateAnalysisGroupModelValidation(IAnalysisGroupRepository analysisGroupRepository,
            ICampaignRepository campaignRepository,
            IClashRepository clashRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Analysis group name must not be empty");

            RuleFor(x => x).Must(x =>
            {
                var entity = analysisGroupRepository.GetByName(x.Name);
                return entity is null || entity.Id == x.Id;
            }).WithMessage("Analysis group name must be unique");

            RuleFor(x => x.Filter)
                .Custom((filter, context) =>
                {
                    if (filter is null)
                    {
                        context.AddFailure(nameof(AnalysisGroup.Filter), "Analysis group filter must not be null");
                        return;
                    }

                    if (!ValidateCriteriaPresence(filter))
                    {
                        context.AddFailure(nameof(AnalysisGroup.Filter), "At least one analysis group filter criteria must be specified");
                        return;
                    }

                    var filterLabels = Mappings.MapToAnalysisGroupFilterSearchModel(mapper.Map<AnalysisGroupFilter>(filter), campaignRepository, clashRepository, productRepository);
                    var invalidFilterValues = new Dictionary<string, List<string>>
                    {
                        {
                            nameof(filter.AdvertiserExternalIds),
                            filter.AdvertiserExternalIds?.Where(id => !filterLabels.Advertisers.ContainsKey(id)).ToList()
                        },
                        {
                            nameof(filter.AgencyExternalIds),
                            filter.AgencyExternalIds?.Where(id => !filterLabels.Agencies.ContainsKey(id)).ToList()
                        },
                        {
                            nameof(filter.AgencyGroupCodes),
                            filter.AgencyGroupCodes?.Where(id => !filterLabels.AgencyGroups.ContainsKey(id)).ToList()
                        },
                        {
                            nameof(filter.BusinessTypes),
                            filter.BusinessTypes?.Where(id => !filterLabels.BusinessTypes.Contains(id)).ToList()
                        },
                        {
                            nameof(filter.CampaignExternalIds),
                            filter.CampaignExternalIds?.Where(id => !filterLabels.Campaigns.ContainsKey(id)).ToList()
                        },
                        {
                            nameof(filter.ClashExternalRefs),
                            filter.ClashExternalRefs?.Where(id => !filterLabels.ClashCodes.ContainsKey(id)).ToList()
                        },
                        {
                            nameof(filter.ProductExternalIds),
                            filter.ProductExternalIds?.Where(id => !filterLabels.Products.ContainsKey(id)).ToList()
                        },
                        {
                            nameof(filter.ReportingCategories),
                            filter.ReportingCategories?.Where(id => !filterLabels.ReportingCategories.Contains(id)).ToList()
                        },
                        {
                            nameof(filter.SalesExecExternalIds),
                            filter.SalesExecExternalIds?.Where(id => !filterLabels.SalesExecs.ContainsKey(id)).Select(x => x.ToString()).ToList()
                        }
                    };

                    foreach (var invalidFilterValue in invalidFilterValues)
                    {
                        if (invalidFilterValue.Value?.Count > 0)
                        {
                            context.AddFailure(nameof(AnalysisGroup.Filter), $"Invalid {invalidFilterValue.Key}: {string.Join(",", invalidFilterValue.Value)}");
                        }
                    }
                });
        }

        private static bool ValidateCriteriaPresence(CreateAnalysisGroupFilterModel x) =>
            x.AdvertiserExternalIds?.Any() == true ||
            x.AgencyExternalIds?.Any() == true ||
            x.AgencyGroupCodes?.Any() == true ||
            x.BusinessTypes?.Any() == true ||
            x.CampaignExternalIds?.Any() == true ||
            x.ClashExternalRefs?.Any() == true ||
            x.ProductExternalIds?.Any() == true ||
            x.ReportingCategories?.Any() == true ||
            x.SalesExecExternalIds?.Any() == true;
    }
}
