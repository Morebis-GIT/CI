using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using CampaignDbObject = ImagineCommunications.GamePlan.Domain.Campaigns.Objects.Campaign;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign
{
    public class SqlBulkCampaignCreatedOrUpdatedEventHandler : IBulkCampaignCreatedOrUpdatedEventHandler
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;

        private readonly ICampaignCleaner _campaignCleaner;
        private readonly IMapper _mapper;
        private readonly ICampaignPassPrioritiesService _campaignPassPrioritiesService;
        private readonly ISqlServerSalesAreaByNullableIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        private IReadOnlyCollection<string> _existingSalesAreas;
        private ImmutableHashSet<string> _existingDemographics;
        private ImmutableHashSet<string> _programmeCategories { get; set; }
        private ImmutableHashSet<string> _breakTypes { get; set; }
        private ImmutableHashSet<int> _bookingPositionGroups { get; set; }

        public SqlBulkCampaignCreatedOrUpdatedEventHandler(
            ICampaignCleaner campaignCleaner,
            ICampaignRepository campaignRepository,
            IMapper mapper,
            IScenarioRepository scenarioRepository,
            IPassRepository passRepository,
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            ISqlServerSalesAreaByNullableIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache
            )
        {
            _campaignCleaner = campaignCleaner;
            _mapper = mapper;
            _dbContextFactory = dbContextFactory;
            _campaignPassPrioritiesService = new CampaignPassPrioritiesService(
                campaignRepository,
                mapper,
                passRepository,
                scenarioRepository);
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        public void Handle(IBulkCampaignCreatedOrUpdated command)
        {
            HandleWithoutLibraryScenario(command);
            HandleLibraryScenario();
        }

        private void AdjustCampaignPassPriority(CampaignDbObject campaign)
        {
            campaign.CampaignPassPriority = campaign.CampaignPassPriority == 0 && campaign.IncludeOptimisation
                ? (int)PassPriorityType.Include
                : campaign.CampaignPassPriority;
        }

        private ValidateContext CreateValidateContext(IBulkCampaignCreatedOrUpdated command)
        {
            using (var dbContext = _dbContextFactory.Create())
            {
                if (_existingSalesAreas is null)
                {
                    _existingSalesAreas = dbContext
                        .Query<GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesArea>()
                        .Select(r => r.Name)
                        .ToArray();
                }

                if (_existingDemographics is null)
                {
                    _existingDemographics = dbContext
                        .Query<GamePlan.Persistence.SqlServer.Entities.Tenant.Demographic>()
                        .Select(r => r.ExternalRef)
                        .ToImmutableHashSet();
                }

                if (_programmeCategories is null)
                {
                    _programmeCategories = dbContext
                        .Query<GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeCategoryHierarchy>()
                        .Select(r => r.Name)
                        .ToImmutableHashSet();
                }

                if (_breakTypes is null)
                {
                    _breakTypes = dbContext
                        .Query<GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas.MetadataCategory>()
                        .Include(e => e.MetadataValues)
                        .FirstOrDefault(e => e.Name == MetaDataKeys.BreakTypes.ToString())
                        ?.MetadataValues
                        .Select(s => s.Value)
                        .ToImmutableHashSet();
                }

                if (_bookingPositionGroups is null)
                {
                    _bookingPositionGroups =
                        dbContext
                            .Query<GamePlan.Persistence.SqlServer.Entities.Tenant.BookingPositionGroups.
                                BookingPositionGroup>().Select(r => r.GroupId)
                            .ToImmutableHashSet();
                }

                var productExternalIdentifiers = command.Data.Select(r => r.Product)
                    .Distinct()
                    .ToList();

                var products = dbContext.Query<GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product>()
                    .Where(x => productExternalIdentifiers.Contains(x.Externalidentifier))
                    .Select(r => r.Externalidentifier)
                    .ToImmutableHashSet();

                return new ValidateContext
                {
                    ExistingSalesAreas = _existingSalesAreas,
                    ExistingDemographics = _existingDemographics,
                    ProgrammeCategories = _programmeCategories,
                    BookingPositionGroups = _bookingPositionGroups,
                    BreakTypes = _breakTypes,
                    Products = products
                };
            }
        }

        public void HandleWithoutLibraryScenario(IBulkCampaignCreatedOrUpdated command)
        {
            var validateContext = CreateValidateContext(command);

            var campaignExternalIds = command.Data.Select(c => c.ExternalId).ToList();

            _campaignCleaner.ExecuteAsync(campaignExternalIds).GetAwaiter().GetResult();

            var campaigns = new List<CampaignDbObject>();

            foreach (var item in command.Data)
            {
                Validator.ValidateCampaign(item, validateContext);

                var newCampaign = _mapper.Map<CampaignDbObject>(item, opts => opts.UseEntityCache(_salesAreaByIdCache));
                AdjustCampaignPassPriority(newCampaign);
                newCampaign.UpdateDerivedKPIs();

                campaigns.Add(newCampaign);
            }

            using (var dbContext = _dbContextFactory.Create())
            {
                dbContext.AddRange(_mapper.Map<GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.Campaign[]>(campaigns, opts => opts.UseEntityCache(_salesAreaByNameCache)));
                dbContext.SaveChanges();
            }
        }

        public void HandleLibraryScenario()
        {
            _campaignPassPrioritiesService.AddNewCampaignPassPrioritiesToAllScenariosInLibrary();
        }
    }
}
