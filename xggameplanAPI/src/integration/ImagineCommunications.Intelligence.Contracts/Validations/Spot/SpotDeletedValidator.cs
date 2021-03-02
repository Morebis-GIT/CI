using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Spot
{
    public class SpotDeletedValidator : AbstractValidator<ISpotDeleted>
    {
        public SpotDeletedValidator() => RuleFor(r => r.ExternalSpotRef).NotEmpty();
    }
}
