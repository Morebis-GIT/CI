using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Breaks
{
    public class BreakCreatedValidator : AbstractValidator<IBreakCreated>
    {
        public BreakCreatedValidator()
        {
            RuleFor(data => data.ScheduledDate).NotNull().NotEmpty();
            RuleFor(data => data.SalesArea).NotNull().NotEmpty();
            RuleFor(data => data.BreakType).NotNull().NotEmpty();
            RuleFor(data => data.Duration).NotNull().NotEmpty();
            RuleFor(data => data.ReserveDuration).NotNull();
            RuleFor(data => data.ExternalBreakRef).NotNull().NotEmpty();
        }
    }
}
