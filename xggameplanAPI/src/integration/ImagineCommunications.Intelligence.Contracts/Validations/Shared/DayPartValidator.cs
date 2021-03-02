using System.Linq;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class DayPartValidator : AbstractValidator<DayPart>
    {
        public DayPartValidator()
        {
            RuleFor(dp => dp.SpotMaxRatings).GreaterThanOrEqualTo(0).WithMessage("Day Part Spot Max Ratings should be greater than or equal to 0");

            RuleFor(dp => dp.Timeslices).NotEmpty().WithMessage("Time Slices are empty");

            RuleForEach(dp => dp.Timeslices).SetValidator(dp => new TimesliceValidator());

            RuleFor(dp => dp.DesiredPercentageSplit)
                .Must((dp, desiredPercentageSplit) =>
                {
                    return desiredPercentageSplit.Equals(dp.Lengths?.Sum(l => l.DesiredPercentageSplit) ?? 0m);
                })
                .When(dp => dp.Lengths != null && dp.Lengths.Any())
                .WithMessage("Sum of length desired percentage should equal to day part desired percentage");

            RuleFor(dp => dp.CurrentPercentageSplit)
                .Must((dp, currentPercentageSplit) =>
                {
                    return currentPercentageSplit.Equals(dp.Lengths?.Sum(l => l.CurrentPercentageSplit) ?? 0m);
                })
                .When(dp => dp.Lengths != null && dp.Lengths.Any())
                .WithMessage("Sum of length current percentage should equal to day part current percentage");

            RuleFor(dp => dp.CampaignPrice).GreaterThanOrEqualTo(0);
        }
    }
}
