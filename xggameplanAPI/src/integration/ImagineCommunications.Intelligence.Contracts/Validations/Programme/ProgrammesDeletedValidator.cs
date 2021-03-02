using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Programme
{
    public class ProgrammesDeletedValidator : AbstractValidator<IProgrammesDeleted>
    {
        public ProgrammesDeletedValidator()
        {
            RuleFor(r => r.SalesArea).NotEmpty();
            RuleFor(r => r.FromDate).NotEmpty();
            RuleFor(r => r.ToDate).NotEmpty();
        }
    }
}
