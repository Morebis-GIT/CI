using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
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
                FolderHelpers.GetTestDataRootDirectoryName(),
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

            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException(
                    $"Test specific data file {fileName} was not found."
                    );
            }

            var data = File.ReadAllLines(fileName);
            var result = new List<object[]>();

            for (var i = 1; i < data.Length; i++)
            {
                var row = new List<object>();

                foreach (var item in data[i].Split(new[] { ',' }, StringSplitOptions.None))
                {
                    row.Add(item);
                }

                result.Add(row.ToArray());
            }

            return result.ToArray();
        }
    }
}
