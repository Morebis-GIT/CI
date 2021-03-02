using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.SalesArea
{
    public class SalesAreaDeletedValidator : AbstractValidator<ISalesAreaDeleted>
    {
        public SalesAreaDeletedValidator()
        {
            RuleFor(d => d.ShortName).NotEmpty();
        }
    }
}
