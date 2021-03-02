using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Campaign
{
    public class SalesAreaCampaignTargetValidator : AbstractValidator<SalesAreaCampaignTarget>
    {
        public SalesAreaCampaignTargetValidator()
        {
            RuleFor(ct => ct.Multiparts).NotEmpty().WithMessage("Multiparts are missing");

            RuleFor(ct => ct.CampaignTargets).NotEmpty().WithMessage("Campaign Targets are missing");

            RuleForEach(ct => ct.Multiparts)
                .SetValidator(ct => new MultipartValidator())
                .When(ct => ct.Multiparts != null);

            RuleForEach(ct => ct.CampaignTargets)
                .SetValidator(ct => new CampaignTargetValidator())
                .When(ct => ct.CampaignTargets != null);
        }
    }
}
