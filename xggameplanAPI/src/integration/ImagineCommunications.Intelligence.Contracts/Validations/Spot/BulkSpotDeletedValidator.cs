using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Spot
{
    public class BulkSpotDeletedValidator : AbstractValidator<IBulkSpotDeleted>
    {
        public BulkSpotDeletedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new SpotDeletedValidator());
        }
    }
}
