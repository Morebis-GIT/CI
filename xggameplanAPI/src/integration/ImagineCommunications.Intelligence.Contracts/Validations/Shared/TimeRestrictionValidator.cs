using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class TimeRestrictionValidator : AbstractValidator<TimeRestriction>
    {
        public TimeRestrictionValidator()
        {
            RuleFor(tr => tr.StartDateTime)
                .NotEmpty().WithMessage("Start Date is missing").Must((c, obj) => DateTime.Compare(obj.Date, c.EndDateTime.Date) <= 1)
            .WithMessage("Start date should be less or equal than end date");

            RuleFor(tr => tr.EndDateTime).NotEmpty().WithMessage("End Date is missing");

            RuleFor(tr => tr.IsIncludeOrExclude)
                .NotEmpty().WithMessage("Is Include Or Exclude is missing")
                .Matches("^(I|E)$").WithMessage("Invalid Is Include Or Exclude (I/E)");

            RuleFor(tr => tr.DowPattern).NotEmpty().WithMessage("Dow Pattern is missing");

            RuleForEach(tr => tr.DowPattern)
                .NotEmpty()
                .Matches("^\\b(?i)(Sun|Mon|Tue|Wed|Thu|Fri|Sat)\\b$")
                .WithMessage("Invalid Dow Pattern");
        }
    }
}
