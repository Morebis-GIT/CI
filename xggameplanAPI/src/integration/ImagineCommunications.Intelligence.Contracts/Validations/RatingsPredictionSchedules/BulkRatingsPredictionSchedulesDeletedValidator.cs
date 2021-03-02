using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.RatingsPredictionSchedules
{
    public class BulkRatingsPredictionSchedulesDeletedValidator : AbstractValidator<IBulkRatingsPredictionSchedulesDeleted>
    {
        public BulkRatingsPredictionSchedulesDeletedValidator()
        {
            RuleFor(d => d.Data).NotEmpty();
            RuleForEach(d => d.Data).SetValidator(t => new RatingsPredictionSchedulesDeletedValidator());
        }
    }
}
