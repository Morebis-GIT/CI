using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Holidays
{
    public class HolidayCreatedValidator : AbstractValidator<IHolidayCreated>
    {
        public HolidayCreatedValidator()
        {
            RuleForEach(data => data.HolidayDateRanges).Must(t => DateTime.Compare(t.Start.Date, t.End.Date) <= 0).WithMessage("Start date must be earlier or equal than enddate");
            RuleFor(x => x.HolidayType).NotNull().Must(t => Enum.IsDefined(typeof(HolidayType), t));
        }
    }
}
