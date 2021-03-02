using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Demographics
{
    public class DemographicDeletedValidator : AbstractValidator<IDemographicDeleted>
    {
        public DemographicDeletedValidator()
        {
            RuleFor(d => d.ExternalRef).NotEmpty();
        }
    }
}
