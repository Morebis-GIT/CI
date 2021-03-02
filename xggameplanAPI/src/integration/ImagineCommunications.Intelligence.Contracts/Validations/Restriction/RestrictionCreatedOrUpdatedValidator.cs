using System.Text.RegularExpressions;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Restriction
{
    public class RestrictionCreatedOrUpdatedValidator : AbstractValidator<IRestrictionCreatedOrUpdated>
    {
        public RestrictionCreatedOrUpdatedValidator()
        {
            RuleFor(r => r.StartDate).NotEmpty();

            RuleFor(r => r)
                .Must(x => !x.EndDate.HasValue || x.EndDate >= x.StartDate)
                .WithMessage("Restriction start date should be less than or equal to end date.")
                .OverridePropertyName("EndDate");

            RuleFor(r => r.RestrictionDays)
                .NotEmpty()
                .Custom((days, context) =>
                {
                    const string zeroOrOne = "^(?!0{7})[0-1]{7}$";

                    var result = !Regex.IsMatch(days, zeroOrOne);
                    if (result)
                    {
                        context.AddFailure("Invalid Restriction Days");
                    }
                });
        }
    }
}
