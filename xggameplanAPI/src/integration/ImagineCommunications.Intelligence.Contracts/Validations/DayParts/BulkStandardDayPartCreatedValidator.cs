using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.DayParts
{
    public class BulkStandardDayPartCreatedValidator : AbstractValidator<IBulkStandardDayPartCreated>
    {
        public BulkStandardDayPartCreatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new StandardDayPartCreatedValidator());
        }
    }
}
