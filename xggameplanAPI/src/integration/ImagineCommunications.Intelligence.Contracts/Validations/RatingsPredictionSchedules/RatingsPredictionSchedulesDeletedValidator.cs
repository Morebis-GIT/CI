using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.RatingsPredictionSchedules
{
    public class RatingsPredictionSchedulesDeletedValidator : AbstractValidator<IRatingsPredictionSchedulesDeleted>
    {
        public RatingsPredictionSchedulesDeletedValidator()
        {
            RuleFor(d => d.SalesArea).NotEmpty();
            RuleFor(d => d.DateTimeFrom).NotEmpty();
            RuleFor(d => d.DateTimeTo).NotEmpty();
            RuleFor(d => d)
                .Must(k => DateTime.Compare(k.DateTimeFrom.Date, k.DateTimeTo.Date) <= 0)
                .OverridePropertyName("DateFromLessOrEqualThanDateTo");
        }
    }
}
