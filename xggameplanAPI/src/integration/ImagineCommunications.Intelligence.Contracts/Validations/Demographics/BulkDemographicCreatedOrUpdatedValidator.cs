using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Demographics
{
    public class BulkDemographicCreatedOrUpdatedValidator : AbstractValidator<IBulkDemographicCreatedOrUpdated>
    {
        public BulkDemographicCreatedOrUpdatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new DemographicCreatedOrUpdatedValidator());
        }
    }
}
