using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Demographics
{
    public class DemographicCreatedOrUpdatedValidator : AbstractValidator<IDemographicCreatedOrUpdated>
    {
        public DemographicCreatedOrUpdatedValidator()
        {
            RuleFor(demos => demos).NotEmpty().WithMessage("Demographics cannot be null");
            RuleFor(demos => demos.ExternalRef).NotEmpty().WithMessage("ExternalRef is not set");
            RuleFor(demos => demos.Name).NotEmpty().WithMessage("Name is not set");
            RuleFor(demos => demos.ShortName).NotEmpty().WithMessage("Short Name is not set");
        }
    }
}
