using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction
{
    public class RestrictionDeletedValidator : AbstractValidator<IRestrictionDeleted>
    {
        public RestrictionDeletedValidator() => RuleFor(x => x.ExternalReference).NotEmpty();
    }
}
