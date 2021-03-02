using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Spot
{
    public class SpotCreatedOrUpdatedValidator : AbstractValidator<ISpotCreatedOrUpdated>
    {
        public SpotCreatedOrUpdatedValidator()
        {
            RuleFor(r => r.ExternalSpotRef).NotEmpty();
            RuleFor(r => r.EndDateTime).NotEmpty();
            RuleFor(r => r.NominalPrice).GreaterThanOrEqualTo(0);

            RuleFor(r => r)
                .Must(x => x.EndDateTime >= x.StartDateTime)
                .WithMessage("Spot EndDateTime cannot be less than StartDateTime")
                .OverridePropertyName("EndDateTime");
        }
    }
}
