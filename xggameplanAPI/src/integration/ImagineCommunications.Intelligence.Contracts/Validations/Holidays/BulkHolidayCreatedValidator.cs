using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Holidays
{
    public class BulkHolidayCreatedValidator : AbstractValidator<IBulkHolidayCreated>
    {
        public BulkHolidayCreatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new HolidayCreatedValidator());
        }
    }
}
