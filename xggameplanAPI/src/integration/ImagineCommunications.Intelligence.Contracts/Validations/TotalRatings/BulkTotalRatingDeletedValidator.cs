using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.TotalRatings
{
    public class BulkTotalRatingDeletedValidator : AbstractValidator<IBulkTotalRatingDeleted>
    {
        public BulkTotalRatingDeletedValidator()
        {
            RuleFor(d => d.Data).NotEmpty();
            RuleForEach(d => d.Data).SetValidator(t => new TotalRatingDeletedValidator());
        }
    }
}
