using System;
using FluentValidation;
using xggameplan.Model;

namespace xggameplan.Validations.DeliveryCappingGroup
{
    public class DeliveryCappingGroupValidation: AbstractValidator<DeliveryCappingGroupModel>
    {
        public DeliveryCappingGroupValidation(Func<DeliveryCappingGroupModel, bool> uniqueDescriptionPredicate)
        {
            RuleFor(p => p.Description).NotEmpty().WithMessage("Description is missing");

            RuleFor(x => x).Must(uniqueDescriptionPredicate).WithMessage("Duplicated description");

            RuleFor(p => p.Percentage).InclusiveBetween(1, 999).WithMessage("Percentage should be in range from 1 to 999");
        }
    }
}
