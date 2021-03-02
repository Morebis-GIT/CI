using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.DayPartGroups
{
    public class BulkStandardDayPartGroupCreatedValidator : AbstractValidator<IBulkStandardDayPartGroupCreated>
    {
        public BulkStandardDayPartGroupCreatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new StandardDayPartGroupCreatedValidator());
        }
    }
}
