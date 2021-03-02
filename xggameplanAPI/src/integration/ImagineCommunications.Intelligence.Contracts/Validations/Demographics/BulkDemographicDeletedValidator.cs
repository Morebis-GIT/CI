using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Demographics
{
    public class BulkDemographicDeletedValidator : AbstractValidator<IBulkDemographicDeleted>
    {
        public BulkDemographicDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new DemographicDeletedValidator());
        }
    }
}
