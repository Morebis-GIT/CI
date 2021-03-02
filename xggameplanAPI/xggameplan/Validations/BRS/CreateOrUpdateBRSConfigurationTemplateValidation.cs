using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.BRS;
using xggameplan.core.Helpers;
using xggameplan.model.External;

namespace xggameplan.Validations.BRS
{
    public class CreateOrUpdateBRSConfigurationTemplateValidation : AbstractValidator<CreateOrUpdateBRSConfigurationTemplateModel>
    {
        private readonly IKPIPriorityRepository _kpiPriorityRepostitory;
        private readonly IBRSConfigurationTemplateRepository _brsConfigurationTemplateRepository;

        public CreateOrUpdateBRSConfigurationTemplateValidation(IKPIPriorityRepository kpiPriorityRepository, IBRSConfigurationTemplateRepository brsConfigurationTemplateRepository)
        {
            _kpiPriorityRepostitory = kpiPriorityRepository;
            _brsConfigurationTemplateRepository = brsConfigurationTemplateRepository;

            RuleFor(x => x).Must(model =>
            {
                var existsItem = _brsConfigurationTemplateRepository.GetByName(model.Name);
                return existsItem == null || existsItem.Id == model.Id;
            }).WithMessage("Duplicated template name");
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is missing");
            RuleFor(p => p.KPIConfigurations).Custom((kpiConfigurations, context) =>
            {
                if (IsEmpty(kpiConfigurations))
                {
                    context.AddFailure("KPIConfigurations are missing");
                    return;
                }

                if (!IsValidKPINames(kpiConfigurations))
                {
                    context.AddFailure($"Expected list of configurations for kpis: {string.Join($",{Environment.NewLine}", BRSHelper.KPIs)}");
                }

                var allKpiPriorities = _kpiPriorityRepostitory.GetAll();

                var excludePriorityId = allKpiPriorities.First(x => x.WeightingFactor == 0).Id;
                var existsPriorities = allKpiPriorities.Select(x => x.Id);
                var inputPriorities = kpiConfigurations.Select(x => x.PriorityId).ToList();

                var isAllPrioritiesExists = !inputPriorities.Except(existsPriorities).Any();
                if (!isAllPrioritiesExists)
                {
                    context.AddFailure("Invalid kpi priorities");
                }

                var isNotAllPrioritiesExclude = inputPriorities.Any(id => id != excludePriorityId);
                if (!isNotAllPrioritiesExclude)
                {
                    context.AddFailure("All kpi configurations are excluded");
                }
            });
        }

        private bool IsEmpty(List<BRSConfigurationForKPIModel> kpiConfigurations) => kpiConfigurations == null || kpiConfigurations.Count == 0;

        private bool IsValidKPINames(List<BRSConfigurationForKPIModel> kpiConfigurations)
        {
            var kpis = kpiConfigurations.Select(x => x.KPIName);
            return kpis.Intersect(BRSHelper.KPIs).Count() == BRSHelper.KPIs.Count;
        }
    }
}
