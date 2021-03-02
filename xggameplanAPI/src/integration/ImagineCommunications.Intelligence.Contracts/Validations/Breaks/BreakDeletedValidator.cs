using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Breaks
{
    public class BreakDeletedValidator : AbstractValidator<IBreakDeleted>
    {
        public BreakDeletedValidator()
        {
            RuleFor(data => data.DateRangeStart).NotNull().Must(x => x != default);
            RuleFor(data => data.DateRangeEnd).NotNull().Must(x => x != default);
            RuleFor(data => data).Must(x => x.DateRangeStart <= x.DateRangeEnd).OverridePropertyName("StartDateGreaterThanEndDate");
            RuleFor(data => data.SalesAreaNames).NotNull().NotEmpty();
        }
    }
}
