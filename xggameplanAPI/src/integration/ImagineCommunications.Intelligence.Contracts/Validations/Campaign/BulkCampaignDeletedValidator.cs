using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Campaign
{
    public class BulkCampaignDeletedValidator : AbstractValidator<IBulkCampaignDeleted>
    {
        public BulkCampaignDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new CampaignDeletedValidator());
        }
    }
}
