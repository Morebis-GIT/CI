using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Holidays
{
    public class BulkHolidayDeletedValidator : AbstractValidator<IBulkHolidayDeleted>
    {
        public BulkHolidayDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new HolidayDeletedValidator());
        }
    }
}
