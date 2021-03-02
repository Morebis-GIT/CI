using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction
{
    public class BulkRestrictionCreatedOrUpdatedValidator : AbstractValidator<IBulkRestrictionCreatedOrUpdated>
    {
        public BulkRestrictionCreatedOrUpdatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new RestrictionCreatedOrUpdatedValidator());
        }
    }
}
