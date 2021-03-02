using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Universe
{
    public class UniverseCreatedValidator : AbstractValidator<IUniverseCreated>
    {
        public UniverseCreatedValidator()
        {
            RuleFor(r => r.SalesArea).NotEmpty();
            RuleFor(r => r.Demographic).NotEmpty();
            RuleFor(r => r.UniverseValue).NotEmpty();
            RuleFor(r => r.StartDate).NotEmpty().LessThanOrEqualTo(r => r.EndDate);
            RuleFor(r => r.EndDate).NotEmpty();
        }
    }
}
