using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Spot
{
    public class BulkSpotCreatedOrUpdatedValidator : AbstractValidator<IBulkSpotCreatedOrUpdated>
    {
        public BulkSpotCreatedOrUpdatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new SpotCreatedOrUpdatedValidator());
        }
    }
}
