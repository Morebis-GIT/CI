using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Clash
{
    public class ClashDeletedValidator : AbstractValidator<IClashDeleted>
    {
        public ClashDeletedValidator() => RuleFor(r => r.Externalref).NotEmpty();
    }
}
