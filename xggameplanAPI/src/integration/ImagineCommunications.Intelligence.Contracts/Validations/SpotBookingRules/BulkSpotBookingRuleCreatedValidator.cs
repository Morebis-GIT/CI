using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SpotBookingRules
{
    public class BulkSpotBookingRuleCreatedValidator : AbstractValidator<IBulkSpotBookingRuleCreated>
    {
        public BulkSpotBookingRuleCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new SpotBookingRuleCreatedValidator());
        }
    }
}
