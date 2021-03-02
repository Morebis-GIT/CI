using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Breaks
{
    public class BulkBreakDeletedValidator : AbstractValidator<IBulkBreaksDeleted>
    {
        public BulkBreakDeletedValidator()
        {
            RuleFor(x => x.Data).NotNull().NotEmpty();
            RuleForEach(x => x.Data).SetValidator(d => new BreakDeletedValidator());
        }
    }
}
