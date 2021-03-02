using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Validators.ProductAdvertiser;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Validates product advertisers.
    /// </summary>
    /// <seealso cref="xggameplan.core.Validators.ProductAdvertiser.IProductAdvertiserValidator" />
    public class ProductAdvertiserValidator : IProductAdvertiserValidator
    {
        private readonly ISqlServerTenantDbContext _dbContext;

        /// <summary>Initializes a new instance of the <see cref="ProductAdvertiserValidator" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <exception cref="ArgumentNullException">dbContext</exception>
        public ProductAdvertiserValidator(ISqlServerTenantDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>Validates the specified advertiser period infos.</summary>
        /// <param name="advertiserPeriodInfos">The advertiser period infos.</param>
        public void Validate(IReadOnlyCollection<IAdvertiserPeriodInfo> advertiserPeriodInfos)
        {
            if (advertiserPeriodInfos is null || advertiserPeriodInfos.Count == 0)
            {
                return;
            }

            var ids = advertiserPeriodInfos.Select(x => x.ExternalIdentifier).Distinct();
            var productAdvertisers = _dbContext.Query<ProductAdvertiser>().Include(p => p.Advertiser)
                .Where(p => ids.Contains(p.Advertiser.ExternalIdentifier))
                .ToList();

            var invalidAdvertisers = advertiserPeriodInfos.Where(x =>
            {
                return productAdvertisers.Where(p => string.Equals(p.Advertiser.ExternalIdentifier,
                    x.ExternalIdentifier, StringComparison.OrdinalIgnoreCase)).All(p =>
                    !x.Period.Overlays(new DateTimeRange(p.StartDate, p.EndDate),
                        DateTimeRange.CompareStrategy.IgnoreEdges));
            }).Select(x => x.ExternalIdentifier).Distinct().ToArray();

            if (invalidAdvertisers.Length > 0)
            {
                throw new InvalidDataException("Invalid Product Advertiser id: " +
                                               string.Join(",", invalidAdvertisers));
            }
        }
    }
}
