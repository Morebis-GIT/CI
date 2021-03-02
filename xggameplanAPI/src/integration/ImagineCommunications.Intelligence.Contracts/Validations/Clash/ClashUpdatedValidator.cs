using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Clash
{
    public class ClashUpdatedValidator : AbstractValidator<IClashUpdated>
    {
        public ClashUpdatedValidator()
        {
            RuleFor(r => r.Externalref).NotEmpty();
            RuleFor(r => r.Description).NotEmpty();

            RuleFor(r => r.Externalref.Length).LessThanOrEqualTo(6);
            RuleFor(r => r.ExposureCount).GreaterThanOrEqualTo(1);

            RuleFor(r => r)
                .Must(clash => !clash.Externalref.Equals(clash.ParentExternalidentifier, StringComparison.OrdinalIgnoreCase))
                .WithMessage(clash => $"Invalid parent clash code at Clash code {clash.Externalref}").OverridePropertyName("SameParentExternalRef");
        }
    }
}
