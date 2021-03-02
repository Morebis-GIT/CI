using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SharedModels;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.SharedValidations;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ClashException
{
    public class ClashExceptionCreatedValidator : AbstractValidator<IClashExceptionCreated>
    {
        public ClashExceptionCreatedValidator()
        {
            RuleFor(d => d.FromValue).NotEmpty();
            RuleFor(d => d.ToValue).NotEmpty();
            RuleFor(d => d.TimeAndDows).NotEmpty();

            RuleFor(d => d.StartDate).NotEmpty();

            RuleFor(d => d)
                .Must(x => x.EndDate == null || DateTime.Compare(x.StartDate.Date, (DateTime)x.EndDate.Value.Date) <= 0)
                .WithMessage("EndDate should be greater than or equal to StartDate if not null.")
                .OverridePropertyName("EndDate");

            RuleFor(d => d)
                .Must(x => x.FromType != ClashExceptionType.Advertiser || x.ToType == ClashExceptionType.Advertiser)
                .WithMessage("If from type is advertiser, to type should also be.")
                .OverridePropertyName("FromToTypeAdvertiser");

            RuleForEach(d => d.TimeAndDows).SetValidator(t => new TimeAndDowValidator());
        }
    }
}
