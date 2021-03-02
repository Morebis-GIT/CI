using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Holidays
{
    public class HolidayDeletedValidator : AbstractValidator<IHolidayDeleted>
    {
        public HolidayDeletedValidator()
        {
            RuleFor(d => d.StartDate).NotEmpty();
            RuleFor(d => d.EndDate).NotEmpty();

        }
    }
}
