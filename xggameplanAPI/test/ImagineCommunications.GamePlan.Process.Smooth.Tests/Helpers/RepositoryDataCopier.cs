using System.Linq;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    /// <summary>
    /// Copies data from source to destination repository for Smooth unit tests
    /// </summary>
    public class RepositoryDataCopier
    {
        private readonly IRepositoryFactory _srcRepositoryFactory;
        private readonly IRepositoryFactory _dstRepositoryFactory;

        /// <summary>
        /// Initialise an new instances of the
        /// <see cref="RepositoryDataCopier"/> class.
        /// </summary>
        /// <param name="sourceRepositoryFactory">
        /// A repository factory for the source data to copy.
        /// </param>
        /// <param name="destinationRepositoryFactory">
        /// A repository factory for the destination of the copied data.
        /// </param>
        public RepositoryDataCopier(
            IRepositoryFactory sourceRepositoryFactory, IRepositoryFactory destinationRepositoryFactory) =>
            (_srcRepositoryFactory, _dstRepositoryFactory) = (sourceRepositoryFactory, destinationRepositoryFactory);

        /// <summary>
        /// Copy data from the source repository to the destination repository.
        /// All data is deleted from the destination repository before copying.
        /// </summary>
        /// <param name="loadAllCoreData">
        /// When <see langword="true"/>, load all core data, otherwise only load
        /// settings and metadata.
        /// </param>
        public void CopyDataToCleanDestination(bool loadAllCoreData)
        {
            CopyData(truncateRepositoriesBeforeLoadingData: true, loadAllCoreData);
        }

        /// <summary>
        /// Copy data from the source repository to the destination repository.
        /// </summary>
        public void CopyDataToExistingDestination()
        {
            CopyData();
        }

        /// <summary>
        /// Copies all data from source to destination repostory.
        /// </summary>
        private void CopyData(
            bool truncateRepositoriesBeforeLoadingData = false,
            bool loadAllCoreData = true)
        {
            using var srcScope = _srcRepositoryFactory.BeginRepositoryScope();
            using var dstScope = _dstRepositoryFactory.BeginRepositoryScope();

            var srcRepositories = srcScope.CreateRepositories(
                typeof(ICampaignRepository),
                typeof(IDemographicRepository),
                typeof(IIndexTypeRepository),
                typeof(IRestrictionRepository),
                typeof(ISalesAreaRepository),
                typeof(ISmoothConfigurationRepository),
                typeof(ISmoothFailureMessageRepository),
                typeof(ISponsorshipRepository),
                typeof(ITenantSettingsRepository)
            );

            var dstRepositories = dstScope.CreateRepositories(
                typeof(ICampaignRepository),
                typeof(IDemographicRepository),
                typeof(IIndexTypeRepository),
                typeof(IRestrictionRepository),
                typeof(ISalesAreaRepository),
                typeof(ISmoothConfigurationRepository),
                typeof(ISmoothFailureMessageRepository),
                typeof(ISponsorshipRepository),
                typeof(ITenantSettingsRepository)
            );

            // Source repositories
            var srcCampaignRepository = srcRepositories.Get<ICampaignRepository>();
            var srcDemographicRepository = srcRepositories.Get<IDemographicRepository>();
            var srcIndexTypeRepository = srcRepositories.Get<IIndexTypeRepository>();
            var srcRestrictionRepository = srcRepositories.Get<IRestrictionRepository>();
            var srcSalesAreaRepository = srcRepositories.Get<ISalesAreaRepository>();
            var srcSmoothConfigurationRepository = srcRepositories.Get<ISmoothConfigurationRepository>();
            var srcSmoothFailureMessageRepository = srcRepositories.Get<ISmoothFailureMessageRepository>();
            var srcSponsorshipRepository = srcRepositories.Get<ISponsorshipRepository>();
            var srcTenantSettingsRepository = srcRepositories.Get<ITenantSettingsRepository>();

            // Destination repositories
            var dstCampaignRepository = dstRepositories.Get<ICampaignRepository>();
            var dstDemographicRepository = dstRepositories.Get<IDemographicRepository>();
            var dstIndexTypeRepository = dstRepositories.Get<IIndexTypeRepository>();
            var dstRestrictionRepository = dstRepositories.Get<IRestrictionRepository>();
            var dstSalesAreaRepository = dstRepositories.Get<ISalesAreaRepository>();
            var dstSmoothConfigurationRepository = dstRepositories.Get<ISmoothConfigurationRepository>();
            var dstSmoothFailureMessageRepository = dstRepositories.Get<ISmoothFailureMessageRepository>();
            var dstSponsorshipRepository = dstRepositories.Get<ISponsorshipRepository>();
            var dstTenantSettingsRepository = dstRepositories.Get<ITenantSettingsRepository>();

            if (truncateRepositoriesBeforeLoadingData)
            {
                var truncate = new TruncateAllPossibleRepositoryData(_dstRepositoryFactory);
                truncate.Truncate();
            }

            Parallel.Invoke(
                () =>
                {
                    var salesAreas = srcSalesAreaRepository.GetAll().ToList();
                    dstSalesAreaRepository.Update(salesAreas);
                },
                () =>
                {
                    var srcDemographics = srcDemographicRepository.GetAll();
                    dstDemographicRepository.Add(srcDemographics);
                },
                () =>
                {
                    var srcSmoothFailureMessages = srcSmoothFailureMessageRepository.GetAll();
                    dstSmoothFailureMessageRepository.Add(srcSmoothFailureMessages);
                },
                () =>
                {
                    var srcTenantSettings = srcTenantSettingsRepository.Get();
                    if (srcTenantSettings != null)
                    {
                        dstTenantSettingsRepository.AddOrUpdate(srcTenantSettings);
                    }
                },
                () =>
                {
                    var srcSmoothConfiguration = srcSmoothConfigurationRepository.GetById(SmoothConfiguration.DefaultId);
                    if (srcSmoothConfiguration != null)
                    {
                        dstSmoothConfigurationRepository.Add(srcSmoothConfiguration);
                    }
                });

            var srcIndexTypes = srcIndexTypeRepository.GetAll();
            dstIndexTypeRepository.Add(srcIndexTypes);

            var srcRestrictions = srcRestrictionRepository.GetAll();
            dstRestrictionRepository.Add(srcRestrictions);

            var srcCampaigns = srcCampaignRepository.GetAll();
            dstCampaignRepository.Add(srcCampaigns);

            var srcSponsorships = srcSponsorshipRepository.GetAll();
            if (srcSponsorships.Any())
            {
                foreach (var item in srcSponsorships)
                {
                    dstSponsorshipRepository.Add(item);
                }

                dstSponsorshipRepository.SaveChanges();
            }

            if (loadAllCoreData)
            {
                Parallel.Invoke(
                    () => Copy<Break, IBreakRepository>(),
                    () => Copy<Clash, IClashRepository>(),
                    () => Copy<ClashException, IClashExceptionRepository>(),
                    () => Copy<Product, IProductRepository>(),
                    () => Copy<Programme, IProgrammeRepository>(),
                    () => Copy<Spot, ISpotRepository>()
                    );
            }
        }

        private void Copy<TModel, TRepository>()
            where TRepository : class, IRepository<TModel>
        {
            using var sourceScope = _srcRepositoryFactory.BeginRepositoryScope();
            using var destinationScope = _dstRepositoryFactory.BeginRepositoryScope();

            var source = sourceScope.CreateRepository<TRepository>();
            var destination = destinationScope.CreateRepository<TRepository>();

            var data = source.GetAll();
            destination.Add(data);
        }
    }
}
