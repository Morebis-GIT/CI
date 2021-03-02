using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories;
using RestrictionEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions.Restriction;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    /// <summary>
    /// Restriction repository which takes into account
    /// Landmark Product related data by its active periods.
    /// </summary>
    /// <seealso cref="ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories.RestrictionRepository" />
    public class LandmarkRestrictionRepository : RestrictionRepository
    {
        private readonly ISqlServerLongRunningTenantDbContext _dbContext;

        /// <summary>Initializes a new instance of the <see cref="LandmarkRestrictionRepository" /> class.</summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="salesAreaByIdCache">SalesArea entity cache assessor.</param>
        /// <param name="salesAreaByNameCache">SalesArea entity cache assessor.</param>
        /// <param name="mapper">The mapper.</param>
        public LandmarkRestrictionRepository(
            ISqlServerLongRunningTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, salesAreaByIdCache, salesAreaByNameCache, mapper)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Prepares <see cref="RestrictionSearchDto"/> query taking into account
        /// Landmark Product related data by its active periods.
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<RestrictionSearchDto> GetRestrictionSearchQueryable()
        {
            var advertiserSubQuery =
                from productAdvertiser in _dbContext.Query<ProductAdvertiser>()
                join product in _dbContext.Query<Product>() on productAdvertiser.ProductId equals product.Uid
                join restriction in _dbContext.Query<RestrictionEntity>() on product.Externalidentifier equals
                    restriction.ProductCode.ToString()
                join advertiser in _dbContext.Query<Advertiser>() on productAdvertiser.AdvertiserId equals advertiser.Id
                where productAdvertiser.StartDate <= restriction.StartDate &&
                      productAdvertiser.EndDate > restriction.StartDate
                select new { productAdvertiser.ProductId, advertiser, RestrictionId = restriction.Id };

            return
                from restriction in _dbContext.Query<RestrictionEntity>()
                join productJoin in _dbContext.Query<Product>() on restriction.ProductCode.ToString() equals
                    productJoin
                        .Externalidentifier into products
                from product in products.DefaultIfEmpty()
                join clashJoin in _dbContext.Query<Clash>() on restriction.ClashCode equals clashJoin.Externalref
                    into clashes
                from clash in clashes.DefaultIfEmpty()

                let programme = _dbContext.Query<ProgrammeDictionary>()
                    .FirstOrDefault(x => restriction.ExternalProgRef == x.ExternalReference)
                let advertiser = advertiserSubQuery
                    .Where(x => x.ProductId == product.Uid && x.RestrictionId == restriction.Id)
                    .Select(x => x.advertiser)
                    .FirstOrDefault()

                select new RestrictionSearchDto
                {
                    Id = restriction.Id,
                    Uid = restriction.Uid,
                    StartDate = restriction.StartDate,
                    EndDate = restriction.EndDate,
                    StartTime = restriction.StartTime,
                    EndTime = restriction.EndTime,
                    RestrictionDays = restriction.RestrictionDays,
                    SchoolHolidayIndicator = restriction.SchoolHolidayIndicator,
                    PublicHolidayIndicator = restriction.PublicHolidayIndicator,
                    LiveProgrammeIndicator = restriction.LiveProgrammeIndicator,
                    RestrictionType = restriction.RestrictionType,
                    RestrictionBasis = restriction.RestrictionBasis,
                    ExternalProgRef = restriction.ExternalProgRef,
                    ProgrammeCategory = restriction.ProgrammeCategory,
                    ProgrammeClassification = restriction.ProgrammeClassification,
                    ProgrammeClassificationIndicator = restriction.ProgrammeClassificationIndicator,
                    TimeToleranceMinsBefore = restriction.TimeToleranceMinsBefore,
                    TimeToleranceMinsAfter = restriction.TimeToleranceMinsAfter,
                    IndexType = restriction.IndexType,
                    IndexThreshold = restriction.IndexThreshold,
                    ProductCode = restriction.ProductCode,
                    ClashCode = restriction.ClashCode,
                    ClearanceCode = restriction.ClearanceCode,
                    ClockNumber = restriction.ClockNumber,
                    ExternalIdentifier = restriction.ExternalIdentifier,
                    SalesAreas = restriction.SalesAreas,
                    ProgrammeDescription = programme.Name,
                    ProductDescription = product.Name,
                    AdvertiserName = advertiser.Name,
                    ClashDescription = clash.Description
                };
        }
    }
}
