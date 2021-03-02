using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SalesArea
{
    public class BulkSalesAreaDeletedValidator : AbstractValidator<IBulkSalesAreaDeleted>
    {
        public BulkSalesAreaDeletedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(data => new SalesAreaDeletedValidator());
        }
    }
}
