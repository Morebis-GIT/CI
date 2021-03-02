using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class TimesliceValidator : AbstractValidator<Timeslice>
    {
        private const string TimeFormat = "^(0?[0-9]|1[0-9]|2[0-3]):([0-5][0-9])(:[0-5][0-9])?$";

        public TimesliceValidator()
        {
            _ = RuleFor(ts => ts.FromTime)
                .NotEmpty().WithMessage("Time Slice From Time is missing")
                .Matches(TimeFormat).WithMessage("Invalid Time Slice From Time");

            _ = RuleFor(ts => ts.ToTime)
                .NotEmpty().WithMessage("Time Slice To Time is missing")
                .Matches(TimeFormat).WithMessage("Invalid Time Slice To Time");

            _ = RuleFor(ts => ts.DowPattern).NotEmpty().WithMessage("Dow Pattern is missing");

            _ = RuleForEach(ts => ts.DowPattern)
                .NotEmpty()
                .Matches("^\\b(?i)(Sun|Mon|Tue|Wed|Thu|Fri|Sat)\\b$")
                .WithMessage("Invalid Dow Pattern");
        }
    }
}
