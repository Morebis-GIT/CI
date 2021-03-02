using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SalesArea
{
    public class BulkSalesAreaCreatedOrUpdatedValidator : AbstractValidator<IBulkSalesAreaCreatedOrUpdated>
    {
        public BulkSalesAreaCreatedOrUpdatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new SalesAreaCreatedOrUpdatedValidator());
        }
    }
}
