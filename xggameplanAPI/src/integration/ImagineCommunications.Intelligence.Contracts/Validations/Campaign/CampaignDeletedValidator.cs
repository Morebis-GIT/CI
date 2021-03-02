using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Campaign
{
    public class CampaignDeletedValidator : AbstractValidator<ICampaignDeleted>
    {
        public CampaignDeletedValidator() => RuleFor(r => r.ExternalId).NotEmpty();
    }
}
