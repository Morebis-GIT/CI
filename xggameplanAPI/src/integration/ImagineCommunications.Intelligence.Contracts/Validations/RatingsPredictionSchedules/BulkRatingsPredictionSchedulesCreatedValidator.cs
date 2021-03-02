using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.RatingsPredictionSchedules
{
    public class BulkRatingsPredictionSchedulesCreatedValidator : AbstractValidator<IBulkRatingsPredictionSchedulesCreated>
    {
        public BulkRatingsPredictionSchedulesCreatedValidator()
        {
            RuleFor(d => d.Data).NotEmpty();

            RuleForEach(d => d.Data).SetValidator(t => new RatingsPredictionSchedulesCreatedValidator());
        }
    }
}
