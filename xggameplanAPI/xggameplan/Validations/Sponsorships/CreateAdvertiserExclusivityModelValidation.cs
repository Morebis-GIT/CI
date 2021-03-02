using ImagineCommunications.GamePlan.Domain.Shared.Products;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateAdvertiserExclusivityModelValidation : AdvertiserExclusivityModelBaseValidation<CreateAdvertiserExclusivityModel>
    {
        public CreateAdvertiserExclusivityModelValidation(IProductRepository productRepository,
                                                          SponsoredItemModelBase sponsoredItemModelBase = null)
                                                        : base(productRepository, sponsoredItemModelBase)
        {
        }
    }
}
