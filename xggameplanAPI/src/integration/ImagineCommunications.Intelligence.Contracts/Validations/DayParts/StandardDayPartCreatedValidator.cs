using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.DayParts
{
    public class StandardDayPartCreatedValidator : AbstractValidator<IStandardDayPartCreated>
    {
        public StandardDayPartCreatedValidator()
        {
            RuleFor(d => d.Name).NotEmpty();
            RuleFor(d => d.SalesArea).NotEmpty();
            RuleFor(d => d.DayPartId).NotEmpty();
            RuleFor(d => d.Order).NotEmpty();
            RuleFor(d => d.Timeslices).NotEmpty();
        }
    }
}
