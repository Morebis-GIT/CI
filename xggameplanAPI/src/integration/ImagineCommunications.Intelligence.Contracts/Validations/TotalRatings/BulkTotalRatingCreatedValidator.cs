using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.TotalRatings
{
    public class BulkTotalRatingCreatedValidator : AbstractValidator<IBulkTotalRatingCreated>
    {
        public BulkTotalRatingCreatedValidator()
        {
            RuleFor(d => d.Data).NotEmpty();
            RuleForEach(d => d.Data).SetValidator(t => new TotalRatingCreatedValidator());
        }
    }
}
