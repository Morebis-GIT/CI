using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class AdvertiserExclusivityModelBaseValidation<T> : ExclusivityModelBaseValidation<T>
    where T : AdvertiserExclusivityModelBase
    {
        private readonly IProductRepository _productRepository;

        public AdvertiserExclusivityModelBaseValidation(IProductRepository productRepository,
                                                        SponsoredItemModelBase sponsoredItemModelBase = null)
                                                      : base(sponsoredItemModelBase)
        {
            _productRepository = productRepository;

            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid AdvertiserExclusivity is required");

            When(model => model != null, () =>
            {
                RuleFor(model => model.AdvertiserIdentifier)
                                      .Must(ContainAdvertiserIdentifier)
                                      .WithMessage("AdvertiserIdentifier is required");

                When(ContainsAdvertiserIdentifier, () =>
                {
                    RuleFor(model => model.AdvertiserIdentifier)
                                          .Must(ContainOnlyExistingAdvertiserIdentifier)
                                          .WithMessage(model => $"AdvertiserIdentifier: {model.AdvertiserIdentifier} do not exists");
                });
            });
        }

        private bool ContainAdvertiserIdentifier(string advertiserIdentifier)
        {
            return !string.IsNullOrWhiteSpace(advertiserIdentifier);
        }

        private bool ContainsAdvertiserIdentifier(T model)
        {
            return ContainAdvertiserIdentifier(model.AdvertiserIdentifier);
        }

        private bool ContainOnlyExistingAdvertiserIdentifier(string advertiserIdentifier)
        {
            return _productRepository.Exists(a => a.AdvertiserIdentifier == advertiserIdentifier);
        }
    }
}
