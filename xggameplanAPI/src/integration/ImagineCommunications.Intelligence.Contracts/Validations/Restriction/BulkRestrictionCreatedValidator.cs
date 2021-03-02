using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction
{
    public class BulkRestrictionCreatedValidator : AbstractValidator<IBulkRestrictionCreatedOrUpdated>
    {
        public BulkRestrictionCreatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new RestrictionCreatedValidator());
        }
    }
}
