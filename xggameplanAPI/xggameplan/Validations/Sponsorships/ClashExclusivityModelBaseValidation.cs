using FluentValidation;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class ClashExclusivityModelBaseValidation<T> : ExclusivityModelBaseValidation<T>
    where T : ClashExclusivityModelBase
    {
        private readonly IClashRepository _clashRepository;

        public ClashExclusivityModelBaseValidation(IClashRepository clashRepository,
                                                   SponsoredItemModelBase sponsoredItemModelBase = null)
                                                 : base(sponsoredItemModelBase)
        {
            _clashRepository = clashRepository;

            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid ClashExclusivity is required");

            When(model => model != null, () =>
            {
                RuleFor(model => model.ClashExternalRef)
                                      .Must(ContainClashExternalRef)
                                      .WithMessage("ClashExternalRef is required");
            });
        }

        private bool ContainClashExternalRef(string clashExternalRef)
        {
            return !string.IsNullOrWhiteSpace(clashExternalRef);
        }

        private bool ContainsClashExternalRef(T model)
        {
            return ContainClashExternalRef(model.ClashExternalRef);
        }
    }
}
