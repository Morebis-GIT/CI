using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Views.Tenant;
using xggameplan.AuditEvents;
using xggameplan.core.Services;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    public class LandmarkAnalysisGroupCampaignQuery : IAnalysisGroupCampaignQuery
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IAuditEventRepository _auditEventRepository;

        public LandmarkAnalysisGroupCampaignQuery(ISqlServerTenantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Guid> GetAnalysisGroupCampaigns(AnalysisGroupFilter filter)
        {
            var initialQuery = _dbContext.Query<Campaign>()
                .Join(
                    _dbContext.Specific.View<CampaignWithProductRelations>(),
                        c => c.Id,
                        s => s.CampaignId,
                        (campaign, relations) => new { relations, campaign }
                )
                .AsQueryable();

            if (!(filter.CampaignExternalIds is null) && filter.CampaignExternalIds.Count > 0)
            {
                initialQuery = initialQuery.Where(c =>
                        filter.CampaignExternalIds.AsEnumerable().Contains(c.campaign.ExternalId)
                    );
            }

            if (!(filter.BusinessTypes is null) && filter.BusinessTypes.Count > 0)
            {
                initialQuery = initialQuery.Where(c =>
                        filter.BusinessTypes.AsEnumerable().Contains(c.campaign.BusinessType)
                    );
            }

            if (!(filter.AdvertiserExternalIds is null) && filter.AdvertiserExternalIds.Count > 0)
            {
                var advertiserQuery = _dbContext.Query<Advertiser>().Where(s =>
                        filter.AdvertiserExternalIds.AsEnumerable().Contains(s.ExternalIdentifier)
                    );

                initialQuery = initialQuery.Join(advertiserQuery, c => c.relations.AdvertiserId, s => s.Id, (a, d) => a);
            }

            if (!(filter.AgencyGroupCodes is null) && filter.AgencyGroupCodes.Count > 0)
            {
                var agencyGroupQuery = _dbContext.Query<AgencyGroup>().Where(s =>
                        filter.AgencyGroupCodes.AsEnumerable().Contains(s.Code)
                    );

                initialQuery = initialQuery.Join(agencyGroupQuery, c => c.relations.AgencyGroupId, s => s.Id, (a, d) => a);
            }

            if (!(filter.AgencyExternalIds is null) && filter.AgencyExternalIds.Count > 0)
            {
                var agencyQuery = _dbContext.Query<Agency>().Where(s =>
                        filter.AgencyExternalIds.AsEnumerable().Contains(s.ExternalIdentifier)
                    );

                initialQuery = initialQuery.Join(agencyQuery, c => c.relations.AgencyId, s => s.Id, (a, d) => a);
            }

            bool hasClashFilter = !(filter.ClashExternalRefs is null) && filter.ClashExternalRefs.Count > 0;
            bool hasProductFilter = !(filter.ProductExternalIds is null) && filter.ProductExternalIds.Count > 0;
            bool hasReportingCategoriesFilter = !(filter.ReportingCategories is null) && filter.ReportingCategories.Count > 0;

            if (hasProductFilter || hasClashFilter || hasReportingCategoriesFilter)
            {
                var productQuery = _dbContext.Query<Product>();

                var intermediateQuery = initialQuery.Join(
                        productQuery,
                        c => c.relations.ProductId,
                        s => s.Id,
                        (campaign, product) => new { campaign, product }
                    );

                if (hasProductFilter)
                {
                    intermediateQuery = intermediateQuery
                        .Where(s => filter.ProductExternalIds.AsEnumerable().Contains(s.product.Externalidentifier));
                }

                if (hasReportingCategoriesFilter)
                {
                    intermediateQuery = intermediateQuery
                        .Where(s => filter.ReportingCategories.AsEnumerable().Contains(s.product.ReportingCategory));
                }

                if (hasClashFilter)
                {
                    var clashQuery = _dbContext.Query<Clash>().Where(s =>
                            filter.ClashExternalRefs.AsEnumerable().Contains(s.Externalref)
                        );

                    intermediateQuery = intermediateQuery
                        .Join(
                            clashQuery,
                            c => c.product.ClashCode,
                            s => s.Externalref,
                            (campaign, clash) => campaign
                        );
                }

                initialQuery = intermediateQuery.Select(c => c.campaign);
            }

            if (!(filter.SalesExecExternalIds is null) && filter.SalesExecExternalIds.Count > 0)
            {
                var personQuery = _dbContext.Query<Person>()
                    .Where(c => filter.SalesExecExternalIds.AsEnumerable().Contains(c.ExternalIdentifier));

                initialQuery = initialQuery
                    .Join(
                        personQuery,
                        c => c.relations.PersonId,
                        s => s.ExternalIdentifier,
                        (campaign, clash) => campaign
                    );
            }

            return initialQuery.Select(c => c.campaign.Id)
                .Distinct()
                .ToArray();
        }
    }
}
