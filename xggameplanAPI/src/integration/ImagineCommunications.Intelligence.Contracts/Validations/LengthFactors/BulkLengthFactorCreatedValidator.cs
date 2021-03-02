using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.LengthFactors
{
    class BulkLengthFactorCreatedValidator : AbstractValidator<IBulkLengthFactorCreated>
    {
        public BulkLengthFactorCreatedValidator()
        {
            RuleFor(x => x.Data).NotEmpty();
            RuleForEach(x => x.Data).SetValidator(x => new LengthFactorCreatedValidator());
        }
    }
}
