using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.common.Services;
using xggameplan.common.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth
{
    public class SmoothEngine
    {
        private readonly RootFolder _smoothLogFileFolder;
        private readonly IClashExposureCountService _clashExposureCountService;
        private readonly IRepositoryFactory _repositoryFactory;

        public delegate void SmoothBatchComplete(
            object sender,
            SalesArea salesArea,
            DateTime fromDateTime,
            DateTime toDateTime,
            List<Recommendation> recommendations,
            List<SmoothFailure> smoothFailures);

        public delegate void SmoothComplete(
            object sender,
            SalesArea salesArea,
            Exception exception,
            SmoothOutput smoothOutput);

        public event SmoothBatchComplete OnSmoothBatchComplete;
        public event SmoothComplete OnSmoothComplete;

        public SmoothEngine(
            IRepositoryFactory repositoryFactory,
            IClashExposureCountService clashExposureCountService,
            RootFolder smoothLogFileFolder)
        {
            _repositoryFactory = repositoryFactory;
            _smoothLogFileFolder = smoothLogFileFolder;
            _clashExposureCountService = clashExposureCountService;
        }

        public void SmoothSalesAreaForDateTimePeriod(
            Guid runId,
            Guid firstScenarioId,
            SalesArea salesArea,
            DateTime processorDateTime,
            DateTimeRange smoothPeriod,
            ImmutableSmoothData threadSafeCollections,
            Action<string> raiseInfo,
            Action<string> raiseWarning,
            Action<string, Exception> raiseException
            )
        {
            SmoothOutput smoothOutput = null;
            Exception caughtException = null;

            try
            {
                using (MachineLock.Create($"SmoothEngine.Smooth.{salesArea.Name}", new TimeSpan(1, 0, 0)))
                {
                    var worker = new SmoothWorkerForSalesAreaDuringDateTimePeriod(
                        _repositoryFactory,
                        _smoothLogFileFolder,
                        threadSafeCollections,
                        _clashExposureCountService,
                        raiseInfo,
                        raiseWarning,
                        raiseException
                        );

                    // Define handler for worker notification of day complete
                    worker.OnSmoothBatchComplete += (sender, currentFromDateTime, currentToDateTime, recommendations, smoothFailures) =>
                    {
                        // Notify parent
                        OnSmoothBatchComplete?.Invoke(this, salesArea, currentFromDateTime, currentToDateTime, recommendations, smoothFailures);
                    };

                    smoothOutput = worker.ActuallyStartSmoothing(
                        runId,
                        firstScenarioId,
                        processorDateTime,
                        smoothPeriod,
                        salesArea);
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
                Debug.WriteLine(ex.ToString());
            }
            finally
            {
                OnSmoothComplete?.Invoke(this, salesArea, caughtException, smoothOutput);
            }
        }
    }
}
