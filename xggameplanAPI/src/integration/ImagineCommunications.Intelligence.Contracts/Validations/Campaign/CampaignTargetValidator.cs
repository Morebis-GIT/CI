using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Campaign
{
    public class CampaignTargetValidator : AbstractValidator<CampaignTarget>
    {
        public CampaignTargetValidator()
        {
            RuleForEach(ct => ct.StrikeWeights)
                .SetValidator(ct => new StrikeWeightValidator())
                .When(ct => ct.StrikeWeights != null);
        }
    }
}
