using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Generic.Extensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.File.Repositories;
using ImagineCommunications.GamePlan.Persistence.Memory.Repositories;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Xunit.Abstractions;
using static ImagineCommunications.GamePlan.Process.Smooth.SmoothWorkerForSalesAreaDuringDateTimePeriod;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests.Helpers
{
    /// <summary>
    /// Performs Smooth test, smooths one or more programmes.
    /// </summary>
    public class SmoothTest
        : IDisposable
    {
        private readonly ITestOutputHelper _output;

        internal IRepositoryFactory RepositoryFactory { get; }

        internal event SmoothBatchComplete OnSmoothBatchComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothTest"/> class
        /// with the name of a directory containing JSON formatted test data.
        /// </summary>
        /// <param name="output">
        /// An instance of an object that captures debug out.
        /// </param>
        /// <param name="loadAllCoreData">
        /// When <see langword="true"/>, load all core data, otherwise only load
        /// settings and metadata.
        /// </param>
        public SmoothTest(
            Guid repositorySegmentationSalt,
            double objectTTL,
            ITestOutputHelper output,
            bool loadAllCoreData
            )
        {
            _output = output;

            RepositoryFactory = new MemoryRepositoryFactory(
                repositorySegmentationSalt,
                objectTTL
                );

            LoadCoreRepositoryData(loadAllCoreData);

            OnSmoothBatchComplete += (_1, _2, _3, _4, _5) => { /* Null sink */ };
        }

        ///<summary>To Do</summary>
        /// <param name="loadAllCoreData">When <see langword="true"/>, load all
        /// core data, otherwise only load settings and metadata.</param>
        private void LoadCoreRepositoryData(bool loadAllCoreData)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                IRepositoryFactory coreFileRepositoryFactory = new FileRepositoryFactory(
                    CoreRepositoryRootDataFolder
                    );

                var copier = new RepositoryDataCopier(coreFileRepositoryFactory, RepositoryFactory);
                copier.CopyDataToCleanDestination(loadAllCoreData);
            }
            finally
            {
                stopwatch.Stop();
                _output.WriteLine($"Loading core repository data => {stopwatch.ElapsedMilliseconds.ToString()}ms");
            }
        }

        private void DebugLogger(string value)
        {
            _output.WriteLine(value);
        }

        private void DebugLogger(string value, Exception error)
        {
            DebugLogger(value);
            _output.WriteLine(error.Message);
        }

        private static string CoreRepositoryRootDataFolder =>
            Path.Combine(
                RepositoryRootDataFolder,
                "CoreRepositoryData"
                );

        private static string TestRepositoryRootDataFolder =>
            Path.Combine(
                RepositoryRootDataFolder,
                "TestRepositoryData"
                );

        private static string TestCsvRootDataFolder =>
            Path.Combine(
                RepositoryRootDataFolder,
                "CvsTestData"
                );

        private static string RepositoryRootDataFolder =>
            Path.Combine(
                FolderHelpers.TestDataRootDirectoryName,
                "Data"
                );

        private SalesArea GetSalesAreaByName(string salesAreaName)
        {
            if (String.IsNullOrWhiteSpace(salesAreaName))
            {
                throw new ArgumentNullException(nameof(salesAreaName));
            }

            using var scope = RepositoryFactory.BeginRepositoryScope();

            var salesAreaRepository = scope.CreateRepository<ISalesAreaRepository>();

            return salesAreaRepository.FindByName(salesAreaName);
        }

        /// <summary>
        /// <para>
        /// Executes Smooth against one or more programmes using the specific
        /// Smooth configuration.
        /// </para>
        /// <para>
        /// The code initializes the repository by copying the template. Each
        /// programme is then Smoothed and the results are checked. This class
        /// performs general checking and then the external action to perform
        /// specific checked is done.
        /// </para>
        /// <para>
        /// It replicates some of the logic from the SmoothProcessor.Execute()
        /// method that cannot be faked.
        /// </para>
        /// </summary>
        public SmoothOutput Execute(
            string salesAreaName,
            DateTimeRange smoothPeriod)
        {
            using var scope = RepositoryFactory.BeginRepositoryScope();
            using var lockSmoothWorker = new Mutex();

            var smoothSession = new SmoothSession(
                scope,
                salesAreaName);

            IModelLoaders modelLoadingService = new ModelLoaders(RepositoryFactory);
            var threadSafeCollections = SmoothProcessor.LoadSmoothData(
                modelLoadingService,
                new List<string> { salesAreaName },
                smoothPeriod,
                DebugLogger);

            var worker = new SmoothWorkerForSalesAreaDuringDateTimePeriod(
                RepositoryFactory,
                String.Empty,
                threadSafeCollections,
                smoothSession.EffectiveClashExposureCountService,
                DebugLogger,
                DebugLogger,
                DebugLogger);

            worker.OnSmoothBatchComplete += OnSmoothBatchComplete;

            var runId = Guid.NewGuid();
            var firstScenarioId = Guid.NewGuid();
            var processorDateTime = DateTime.UtcNow;
            SalesArea salesArea = GetSalesAreaByName(salesAreaName);

            return worker.ActuallyStartSmoothing(
                runId,
                firstScenarioId,
                processorDateTime,
                smoothPeriod,
                salesArea
                );
        }

        internal void LoadTestSpecificRepositories(
            IRepositoryFactory repositoryFactory,
            string testSpecificRepositoryDataFolderName
            )
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var folder = Path.Combine(
                    TestRepositoryRootDataFolder,
                    testSpecificRepositoryDataFolderName
                    );

                if (!Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException(
                        $"Test specific data folder {folder} was not found."
                        );
                }

                var testFileRepositoryFactory = new FileRepositoryFactory(folder);

                var copier = new RepositoryDataCopier(testFileRepositoryFactory, repositoryFactory);
                copier.CopyDataToExistingDestination();
            }
            finally
            {
                stopwatch.Stop();
                _output.WriteLine($"Loading specific repository data => {stopwatch.ElapsedMilliseconds.ToString()}ms");
            }
        }

        internal static IEnumerable<object[]> LoadTestSpecificCsvFile(
          string testSpecificRepositoryDataFileName
          )
        {
            var fileName = Path.Combine(
                TestCsvRootDataFolder,
                testSpecificRepositoryDataFileName
                );

            string[] data = File.ReadAllLines(fileName);

            // Length - 1 to skip the header row
            var rowCount = data.Length - 1;
            var result = new object[rowCount][];

            for (var i = 1; i < data.Length; i++)
            {
                result[i - 1] = data[i].Split(_separator, StringSplitOptions.None);
            }

            return result;
        }

        private static readonly char[] _separator = new[] { ',' };
        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Delete everything I added to the memory repository.
                    // Not doing this causes massive memory leaks.
                    using var repoScope = RepositoryFactory.BeginRepositoryScope();

                    var repositories = repoScope.CreateRepositories(
                        typeof(IBreakRepository),
                        typeof(ICampaignRepository),
                        typeof(IClashExceptionRepository),
                        typeof(IClashRepository),
                        typeof(IDemographicRepository),
                        typeof(IIndexTypeRepository),
                        typeof(IProductRepository),
                        typeof(IProgrammeRepository),
                        typeof(IRatingsScheduleRepository),
                        typeof(IRecommendationRepository),
                        typeof(IRestrictionRepository),
                        typeof(ISalesAreaRepository),
                        typeof(IScheduleRepository),
                        typeof(ISmoothConfigurationRepository),
                        typeof(ISmoothFailureMessageRepository),
                        typeof(ISmoothFailureRepository),
                        typeof(ISponsorshipRepository),
                        typeof(ISpotPlacementRepository),
                        typeof(ISpotRepository),
                        typeof(ITenantSettingsRepository),
                        typeof(IUniverseRepository)
                    );

                    var breakRepository = repositories.Get<IBreakRepository>();
                    var campaignRepository = repositories.Get<ICampaignRepository>();
                    var clashExceptionRepository = repositories.Get<IClashExceptionRepository>();
                    var clashRepository = repositories.Get<IClashRepository>();
                    var demographicRepository = repositories.Get<IDemographicRepository>();
                    var indexTypeRepository = repositories.Get<IIndexTypeRepository>();
                    var productRepository = repositories.Get<IProductRepository>();
                    var programmeRepository = repositories.Get<IProgrammeRepository>();
                    var ratingScheduleRepository = repositories.Get<IRatingsScheduleRepository>();
                    var recommendationRepository = repositories.Get<IRecommendationRepository>();
                    var restrictionRepository = repositories.Get<IRestrictionRepository>();
                    var salesAreaRepository = repositories.Get<ISalesAreaRepository>();
                    var scheduleRepository = repositories.Get<IScheduleRepository>();
                    var smoothConfigurationRepository = repositories.Get<ISmoothConfigurationRepository>();
                    var smoothFailureMessageRepository = repositories.Get<ISmoothFailureMessageRepository>();
                    var smoothFailureRepository = repositories.Get<ISmoothFailureRepository>();
                    var sponsorshipRepository = repositories.Get<ISponsorshipRepository>();
                    var spotPlacementRepository = repositories.Get<ISpotPlacementRepository>();
                    var spotRepository = repositories.Get<ISpotRepository>();
                    var tenantSettingsRepository = repositories.Get<ITenantSettingsRepository>();
                    var universeRepository = repositories.Get<IUniverseRepository>();

                    breakRepository.Truncate();
                    campaignRepository.Truncate();
                    clashExceptionRepository.Truncate();
                    clashRepository.Truncate();
                    demographicRepository.Truncate();
                    indexTypeRepository.Truncate();
                    productRepository.Truncate();
                    programmeRepository.Truncate();
                    ((MemoryRatingsScheduleRepository)ratingScheduleRepository).Truncate();
                    ((MemoryRecommendationRepository)recommendationRepository).Truncate();
                    restrictionRepository.Truncate();
                    ((MemorySalesAreaRepository)salesAreaRepository).Truncate();
                    scheduleRepository.Truncate();
                    smoothConfigurationRepository.Truncate();
                    smoothFailureMessageRepository.Truncate();
                    smoothFailureRepository.Truncate();
                    ((MemorySponsorshipRepository)sponsorshipRepository).Truncate();
                    ((MemorySpotPlacementRepository)spotPlacementRepository).Truncate();
                    spotRepository.Truncate();
                    tenantSettingsRepository.Truncate();
                    universeRepository.Truncate();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
