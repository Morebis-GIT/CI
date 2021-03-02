using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.TotalRatings
{
    public class TotalRatingDeletedValidator : AbstractValidator<ITotalRatingDeleted>
    {
        public TotalRatingDeletedValidator()
        {
            RuleFor(data => data.DateTimeFrom).NotEmpty();
            RuleFor(data => data.DateTimeTo).NotEmpty();
            RuleFor(data => data.SalesArea).NotEmpty();
            When(c => c.DateTimeFrom != DateTime.MinValue && c.DateTimeTo != DateTime.MinValue, () =>
               RuleFor(data => data.DateTimeFrom).Must((tot, data) => DateTime.Compare(data.Date, tot.DateTimeTo.Date) <= 0)
                   .OverridePropertyName("DateToLessThanDateFrom")
            );
        }
    }
}
