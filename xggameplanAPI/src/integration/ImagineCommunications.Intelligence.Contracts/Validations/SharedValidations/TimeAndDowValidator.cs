using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SharedModels;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SharedValidations
{
    public class TimeAndDowValidator:AbstractValidator<TimeAndDow>
    {
        public TimeAndDowValidator()
        {
            RuleFor(d => d.DaysOfWeek).Matches("^(?!0{7})[0-1]{7}$");
        }
    }
}
