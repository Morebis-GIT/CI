using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.LengthFactors
{
    public class LengthFactorCreatedValidator : AbstractValidator<ILengthFactorCreated>
    {
        public LengthFactorCreatedValidator()
        {
            RuleFor(x => x.SalesArea).NotEmpty().WithMessage("SalesArea is missing");

            RuleFor(x => x.Factor).GreaterThanOrEqualTo(0).WithMessage("Length Factor should be >= 0");
        }
    }
}
