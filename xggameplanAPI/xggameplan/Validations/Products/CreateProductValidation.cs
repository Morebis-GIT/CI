using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateProductValidation : AbstractValidator<CreateProduct>
    {
        public CreateProductValidation()
        {
            RuleFor(p => p.Externalidentifier).NotEmpty().WithMessage("External Id is missing");

            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is missing");

            RuleFor(p => p.EffectiveStartDate).NotEmpty().WithMessage("Effective start date is missing");

            RuleFor(p => p.EffectiveEndDate).NotEmpty().WithMessage("Effective end date is missing");

            RuleFor(p => p)
                .Must(p => p.EffectiveStartDate < p.EffectiveEndDate).WithMessage("Effective start/end dates are overlapping");

            RuleFor(p => p.ClashCode).NotEmpty().WithMessage("Clash code is missing");

            RuleFor(p => p.AdvertiserIdentifier).NotEmpty().WithMessage("Advertiser identifier is missing");

            RuleFor(p => p.AdvertiserName).NotEmpty().WithMessage("Advertiser name is missing");
        }
    }
}
