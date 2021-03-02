using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.DayPartGroups
{
    public class StandardDayPartGroupCreatedValidator : AbstractValidator<IStandardDayPartGroupCreated>
    {
        public StandardDayPartGroupCreatedValidator()
        {
            RuleFor(d => d.GroupId).NotEmpty();
            RuleFor(d => d.SalesArea).NotEmpty();
            RuleFor(d => d.Optimizer).NotNull();
            RuleFor(d => d.Policy).NotNull();
            RuleFor(d => d.RatingReplacement).NotNull();
        }
    }
}
