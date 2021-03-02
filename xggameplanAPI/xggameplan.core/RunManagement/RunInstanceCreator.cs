using System;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.core.BRS;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.RunManagement;

namespace xggameplan.core.RunManagement
{
    public class RunInstanceCreator
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAutoBookInputHandler _autoBookInputHandler;
        private readonly IAutoBookOutputHandler _autoBookOutputHandler;
        private readonly RunCompletionNotifier _runCompletionNotifier;
        private readonly ScenarioSnapshotGenerator _scenarioSnapshotGenerator;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;
        private readonly IBRSIndicatorManager _brsIndicatorManager;
        private readonly ILandmarkRunService _landmarkRunService;
        private readonly IAutoBooks _autoBooks;
        private readonly IConfiguration _configuration;

        public RunInstanceCreator(
            IRepositoryFactory repositoryFactory,
            IAuditEventRepository auditEventRepository,
            IAutoBookInputHandler autoBookInputHandler,
            IAutoBookOutputHandler autoBookOutputHandler,
            RunCompletionNotifier runCompletionNotifier,
            ScenarioSnapshotGenerator scenarioSnapshotGenerator,
            ISynchronizationService synchronizationService,
            IPipelineAuditEventRepository pipelineAuditEventRepository,
            IBRSIndicatorManager brsIndicatorManager,
            ILandmarkRunService landmarkRunService,
            IAutoBooks autoBooks,
            IConfiguration configuration)
        {
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
            _autoBookInputHandler = autoBookInputHandler;
            _autoBookOutputHandler = autoBookOutputHandler;
            _runCompletionNotifier = runCompletionNotifier;
            _scenarioSnapshotGenerator = scenarioSnapshotGenerator;
            _synchronizationService = synchronizationService;
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
            _brsIndicatorManager = brsIndicatorManager;
            _landmarkRunService = landmarkRunService;
            _autoBooks = autoBooks;
            _configuration = configuration;
        }

        /// <summary>
        /// Returns RunInstance for run scenario
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        public RunInstance Create(Guid runId, Guid scenarioId)
        {
            return new RunInstance(runId, scenarioId, _repositoryFactory, _auditEventRepository,
                _autoBookInputHandler, _autoBookOutputHandler, _runCompletionNotifier, _scenarioSnapshotGenerator,
                _synchronizationService, _pipelineAuditEventRepository, _brsIndicatorManager, _landmarkRunService, _autoBooks, _configuration);
        }
    }
}
