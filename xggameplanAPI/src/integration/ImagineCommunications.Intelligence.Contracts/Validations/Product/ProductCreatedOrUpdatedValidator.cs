using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Product
{
    public class ProductCreatedOrUpdatedValidator : AbstractValidator<IProductCreatedOrUpdated>
    {
        public ProductCreatedOrUpdatedValidator()
        {
            RuleFor(data => data.Externalidentifier).NotEmpty().WithMessage("ExternalId must not be null");

            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is missing");

            RuleFor(p => p.EffectiveStartDate).NotEmpty().WithMessage("Effective start date is missing");

            RuleFor(p => p.EffectiveEndDate).NotEmpty().WithMessage("Effective end date is missing");

            RuleFor(p => p)
                .Must(p => p.EffectiveStartDate < p.EffectiveEndDate).WithMessage("Effective start/end dates are overlapping").OverridePropertyName("ProductCreatedOrUpdated_InvalidDateRange");

            RuleFor(p => p.ClashCode).NotEmpty().WithMessage("Clash code is missing");
        }
    }
}
