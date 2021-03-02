using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.RatingsPredictionSchedules
{
    public class RatingsPredictionSchedulesCreatedValidator : AbstractValidator<IRatingsPredictionScheduleCreated>
    {
        public RatingsPredictionSchedulesCreatedValidator()
        {
            RuleFor(d => d.ScheduleDay).NotEmpty();
            RuleFor(d => d.SalesArea).NotEmpty();
        }
    }
}
