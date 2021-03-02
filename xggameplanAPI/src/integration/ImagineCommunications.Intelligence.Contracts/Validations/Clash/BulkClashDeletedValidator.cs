using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Clash
{
    public class BulkClashDeletedValidator : AbstractValidator<BulkClashDeleted>
    {
        public BulkClashDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new ClashDeletedValidator());
        }
    }
}
