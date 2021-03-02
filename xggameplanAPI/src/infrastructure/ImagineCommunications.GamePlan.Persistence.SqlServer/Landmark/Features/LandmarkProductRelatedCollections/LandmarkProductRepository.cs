using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using NodaTime;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Product repository which takes into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    /// <seealso cref="ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories.ProductRepository" />
    public class LandmarkProductRepository : ProductRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;

        /// <summary>Initializes a new instance of the <see cref="LandmarkProductRepository" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="searchConditionBuilder">The search condition builder.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="clock">The clock.</param>
        public LandmarkProductRepository(
            ISqlServerTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            IMapper mapper,
            IClock clock)
            : base(dbContext, searchConditionBuilder, mapper, clock)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Prepares <see cref="ProductDto"/> query taking into account
        /// Landmark Product related data by its active periods.
        /// </summary>
        /// <param name="onDate">The on date.</param>
        /// <param name="productExpressionPredicate">The product expression predicate.</param>
        /// <returns></returns>
        protected override IQueryable<ProductDto> ActualProductQuery(DateTime onDate,
            Expression<Func<Product, bool>> productExpressionPredicate = null)
        {
            var query =
                from product in _dbContext.Query<Entities.Tenant.Products.Product>()
                join productAdvertiserJoin in _dbContext.Query<ProductAdvertiser>() on product.Uid equals
                    productAdvertiserJoin.ProductId
                    into paJoin
                from productAdvertiser in paJoin.Where(x => x.StartDate <= onDate && x.EndDate > onDate)
                    .DefaultIfEmpty()
                join advertiserJoin in _dbContext.Query<Advertiser>() on productAdvertiser.AdvertiserId equals
                    advertiserJoin.Id into aJoin
                from advertiser in aJoin.DefaultIfEmpty()
                join productAgencyJoin in _dbContext.Query<ProductAgency>() on product.Uid equals productAgencyJoin
                        .ProductId
                    into pagJoin
                from productAgency in pagJoin.Where(x => x.StartDate <= onDate && x.EndDate > onDate).DefaultIfEmpty()
                join agencyJoin in _dbContext.Query<Agency>() on productAgency.AgencyId equals agencyJoin.Id into agJoin
                from agency in agJoin.DefaultIfEmpty()
                join agencyGroupJoin in _dbContext.Query<Entities.Tenant.AgencyGroup>() on productAgency.AgencyGroupId
                    equals agencyGroupJoin.Id
                    into aggJoin
                from agencyGroup in aggJoin.DefaultIfEmpty()
                join productPersonJoin in _dbContext.Query<ProductPerson>() on product.Uid equals productPersonJoin
                        .ProductId
                    into pseJoin
                from productPerson in pseJoin.Where(x => x.StartDate <= onDate && x.EndDate > onDate).DefaultIfEmpty()
                join personJoin in _dbContext.Query<Person>() on productPerson.PersonId equals personJoin.Id into seJoin
                from person in seJoin.DefaultIfEmpty()
                select new
                {
                    product,
                    productAdvertiser,
                    productAgency,
                    productPerson,
                    advertiser,
                    agency,
                    agencyGroup,
                    person
                };

            if (!(productExpressionPredicate is null))
            {
                query = ExpressProductPredicate(query, productExpressionPredicate);
            }

            return query.Select(x => new ProductDto
            {
                Uid = x.product.Uid,
                Name = x.product.Name,
                Externalidentifier = x.product.Externalidentifier,
                ParentExternalidentifier = x.product.ParentExternalidentifier,
                ClashCode = x.product.ClashCode,
                EffectiveStartDate = x.product.EffectiveStartDate,
                EffectiveEndDate = x.product.EffectiveEndDate,
                ReportingCategory = x.product.ReportingCategory,
                AdvertiserId = x.advertiser.Id,
                AdvertiserIdentifier = x.advertiser.ExternalIdentifier,
                AdvertiserName = x.advertiser.Name,
                AdvertiserShortName = x.advertiser.ShortName,
                AdvertiserStartDate = x.productAdvertiser.StartDate,
                AdvertiserEndDate = x.productAdvertiser.EndDate,
                AgencyId = x.agency.Id,
                AgencyIdentifier = x.agency.ExternalIdentifier,
                AgencyName = x.agency.Name,
                AgencyShortName = x.agency.ShortName,
                AgencyStartDate = x.productAgency.StartDate,
                AgencyEndDate = x.productAgency.EndDate,
                AgencyGroupId = x.agencyGroup.Id,
                AgencyGroupShortName = x.agencyGroup.ShortName,
                AgencyGroupCode = x.agencyGroup.Code,
                PersonId = x.person.Id,
                PersonIdentifier = x.person.ExternalIdentifier,
                PersonName = x.person.Name,
                PersonStartDate = x.productPerson.StartDate,
                PersonEndDate = x.productPerson.EndDate
            });
        }
    }
}
