using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction
{
    public class BulkRestrictionDeletedValidator : AbstractValidator<IBulkRestrictionDeleted>
    {
        public BulkRestrictionDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new RestrictionDeletedValidator());
        }
    }
}
