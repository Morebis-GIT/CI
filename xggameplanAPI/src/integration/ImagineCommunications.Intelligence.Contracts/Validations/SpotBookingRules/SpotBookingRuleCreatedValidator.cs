using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SpotBookingRules
{
    public class SpotBookingRuleCreatedValidator : AbstractValidator<ISpotBookingRuleCreated>
    {
        public SpotBookingRuleCreatedValidator()
        {
            RuleFor(r => r.MinBreakLength).NotNull();
            RuleFor(r => r.MaxBreakLength).NotNull();
        }
    }
}
