using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Interfaces;
using xggameplan.Model;
using Clash = ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects.Clash;
using Product = ImagineCommunications.GamePlan.Domain.Shared.Products.Objects.Product;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;
using Spot = ImagineCommunications.GamePlan.Domain.Spots.Spot;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    public class LandmarkSpotModelCreator : ISpotModelCreator
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IClashRepository _clashRepository;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public LandmarkSpotModelCreator(
            ICampaignRepository campaignRepository,
            IClashRepository clashRepository,
            ISqlServerTenantDbContext dbContext,
            IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _clashRepository = clashRepository;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<SpotModel> Create(IReadOnlyCollection<Spot> spots)
        {
            if (spots is null || spots.Count == 0)
            {
                return Enumerable.Empty<SpotModel>();
            }

            var spotIds = spots.Select(sp => sp.Uid);

            var spotLinks =
                (from spotWithCampaignAndProductRelations in _dbContext.Specific
                        .View<SpotWithCampaignAndProductRelations>()
                 join productJoin in _dbContext.Query<ProductEntity>() on spotWithCampaignAndProductRelations
                         .ProductId
                     equals productJoin.Id into pJoin
                 from product in pJoin.DefaultIfEmpty()
                 join productAdvertiserJoin in _dbContext.Query<ProductAdvertiser>() on
                     spotWithCampaignAndProductRelations.ProductAdvertiserId equals productAdvertiserJoin.Id
                     into paJoin
                 from productAdvertiser in paJoin.DefaultIfEmpty()
                 join advertiserJoin in _dbContext.Query<Advertiser>() on productAdvertiser.AdvertiserId equals
                     advertiserJoin.Id into aJoin
                 from advertiser in aJoin.DefaultIfEmpty()
                 join productAgencyJoin in _dbContext.Query<ProductAgency>() on spotWithCampaignAndProductRelations
                         .ProductAgencyId equals productAgencyJoin.Id
                     into pagJoin
                 from productAgency in pagJoin.DefaultIfEmpty()
                 join agencyJoin in _dbContext.Query<Agency>() on productAgency.AgencyId equals agencyJoin.Id into
                     agJoin
                 from agency in agJoin.DefaultIfEmpty()
                 join agencyGroupJoin in _dbContext.Query<Entities.Tenant.AgencyGroup>() on productAgency
                         .AgencyGroupId
                     equals agencyGroupJoin.Id
                     into aggJoin
                 from agencyGroup in aggJoin.DefaultIfEmpty()
                 join productPersonJoin in _dbContext.Query<ProductPerson>() on spotWithCampaignAndProductRelations
                         .ProductPersonId equals productPersonJoin.Id
                     into pseJoin
                 from productPerson in pseJoin.DefaultIfEmpty()
                 join personJoin in _dbContext.Query<Person>() on productPerson.PersonId equals personJoin.Id into
                     seJoin
                 from person in seJoin.DefaultIfEmpty()
                 where spotIds.Contains(spotWithCampaignAndProductRelations.SpotUid)
                 select new SpotLink
                 {
                     SpotUid = spotWithCampaignAndProductRelations.SpotUid,
                     CampaignExternalId = spotWithCampaignAndProductRelations.CampaignExternalId,
                     ProductDto = product == null
                         ? null
                         : new ProductDto
                         {
                             Uid = product.Id,
                             Name = product.Name,
                             Externalidentifier = product.Externalidentifier,
                             ParentExternalidentifier = product.ParentExternalidentifier,
                             ClashCode = product.ClashCode,
                             EffectiveStartDate = product.EffectiveStartDate,
                             EffectiveEndDate = product.EffectiveEndDate,
                             ReportingCategory = product.ReportingCategory,
                             AdvertiserId = advertiser.Id,
                             AdvertiserIdentifier = advertiser.ExternalIdentifier,
                             AdvertiserName = advertiser.Name,
                             AdvertiserShortName = advertiser.ShortName,
                             AdvertiserStartDate = productAdvertiser.StartDate,
                             AdvertiserEndDate = productAdvertiser.EndDate,
                             AgencyId = agency.Id,
                             AgencyIdentifier = agency.ExternalIdentifier,
                             AgencyName = agency.Name,
                             AgencyShortName = agency.ShortName,
                             AgencyStartDate = productAgency.StartDate,
                             AgencyEndDate = productAgency.EndDate,
                             AgencyGroupId = agencyGroup.Id,
                             AgencyGroupShortName = agencyGroup.ShortName,
                             AgencyGroupCode = agencyGroup.Code,
                             PersonId = person.Id,
                             PersonIdentifier = person.ExternalIdentifier,
                             PersonName = person.Name,
                             PersonStartDate = productPerson.StartDate,
                             PersonEndDate = productPerson.EndDate
                         }
                 }).AsNoTracking()
                .ToDictionary(k => k.SpotUid);

            var campaignExternalIds = spotLinks.Values.Select(s => s.CampaignExternalId)
                .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var campaigns =
                (campaignExternalIds.Count == 0
                    ? Enumerable.Empty<Campaign>()
                    : _campaignRepository.FindByRefs(campaignExternalIds))
                .ToDictionary(k => k.ExternalId.ToUpperInvariant());

            var clashExternalIds = spotLinks.Values.Select(x => x.ProductDto?.ClashCode)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase).ToList();

            var clashes =
                (clashExternalIds.Count == 0
                    ? Enumerable.Empty<Clash>()
                    : _clashRepository.FindByExternal(clashExternalIds))
                .GroupBy(k => k.Externalref, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key.ToUpperInvariant(), g => g.ToList());

            return spots.Select(spot =>
            {
                _ = spotLinks.TryGetValue(spot.Uid, out var spotLink);

                Campaign campaign = null;
                _ = !string.IsNullOrWhiteSpace(spotLink?.CampaignExternalId) &&
                                     campaigns.TryGetValue(spotLink.CampaignExternalId.ToUpperInvariant(),
                                         out campaign);

                List<Clash> clashList = null;
                _ = !string.IsNullOrWhiteSpace(spotLink?.ProductDto?.ClashCode) &&
                                   clashes.TryGetValue(spotLink.ProductDto.ClashCode.ToUpperInvariant(), out clashList);

                return _mapper.Map<SpotModel>(Tuple.Create(spot, campaign,
                    spotLink?.ProductDto is null ? null : _mapper.Map<Product>(spotLink.ProductDto), clashList));
            });
        }

        private class SpotLink
        {
            public Guid SpotUid { get; set; }
            public string CampaignExternalId { get; set; }
            public ProductDto ProductDto { get; set; }
        }
    }
}
