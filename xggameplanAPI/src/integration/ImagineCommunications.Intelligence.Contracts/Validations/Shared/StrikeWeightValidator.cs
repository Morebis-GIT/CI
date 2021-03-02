using System.Linq;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class StrikeWeightValidator : AbstractValidator<StrikeWeight>
    {
        public StrikeWeightValidator()
        {
            RuleFor(sw => sw.StartDate)
                .NotEmpty().WithMessage("Strike Weight Start Date is missing")
                .LessThanOrEqualTo(sw => sw.EndDate).WithMessage("Strike weight start date should be less than or equal to end date");

            RuleFor(sw => sw.EndDate).NotEmpty().WithMessage("Strike Weight End Date is missing");

            RuleFor(sw => sw.SpotMaxRatings).GreaterThanOrEqualTo(0).WithMessage("Strike Weight Spot Max Ratings should be greater than or equal to 0");

            RuleFor(sw => sw.DayParts).NotEmpty().WithMessage("Day Parts are missing");

            RuleFor(sw => sw.DesiredPercentageSplit)
                .Must((sw, desiredPercentageSplit) =>
                {
                    return desiredPercentageSplit.Equals(sw.DayParts?.Sum(dp => dp?.DesiredPercentageSplit ?? 0m));
                }).WithMessage("Sum of day part desired percentage should equal to strike weight desired percentage");

            RuleFor(sw => sw.CurrentPercentageSplit)
                .Must((sw, currentPercentageSplit) =>
                {
                    return currentPercentageSplit.Equals(sw.DayParts?.Sum(dp => dp?.CurrentPercentageSplit ?? 0m));
                }).WithMessage("Sum of day part current percentage should equal to strike weight current percentage");

            RuleForEach(sw => sw.DayParts).SetValidator(sw => new DayPartValidator());

            When(sw => sw.Lengths != null && sw.Lengths.Any(), () =>
            {
                RuleForEach(sw => sw.Lengths)
                    .SetValidator(sw => new LengthValidator());
            });
        }
    }
}
