using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using xggameplan.core.Validators.ProductAdvertiser;
using xggameplan.Extensions;

namespace xggameplan.core.Validators
{
    public class ProductAdvertiserValidator : IProductAdvertiserValidator
    {
        private readonly IProductRepository _productRepository;

        public ProductAdvertiserValidator(IProductRepository productRepository)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public void Validate(IReadOnlyCollection<IAdvertiserPeriodInfo> advertiserPeriodInfos)
        {
            if (advertiserPeriodInfos is null || advertiserPeriodInfos.Count == 0)
            {
                return;
            }

            var ids = advertiserPeriodInfos.Select(x => x.ExternalIdentifier)
                .Distinct(StringComparer.CurrentCultureIgnoreCase).Trim().ToList();
            var products = _productRepository.FindByAdvertiserId(ids)?.ToList();

            var invalidProductAdvertiserIds = ids.Except(
                products?.Select(p => p.AdvertiserIdentifier)?.Distinct() ?? Enumerable.Empty<string>(),
                StringComparer.CurrentCultureIgnoreCase).ToList();

            if (invalidProductAdvertiserIds.Any())
            {
                throw new InvalidDataException("Invalid Product Advertiser id: " +
                                               string.Join(",", invalidProductAdvertiserIds.ToList()));
            }
        }
    }
}
