using System;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures;
using ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.AuditEvents;
using xggameplan.OutputProcessors.Abstractions;

namespace xggameplan.core.OutputProcessors.DataHandlers
{
    public class FailuresDataHandler : IOutputDataHandler<Failures>
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IAuditEventRepository _auditEventRepository;

        public FailuresDataHandler(IRepositoryFactory repositoryFactory, IAuditEventRepository auditEventRepository)
        {
            _repositoryFactory = repositoryFactory;
            _auditEventRepository = auditEventRepository;
        }

        public void ProcessData(Failures data, Run run, Scenario scenario)
        {
            try
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Failures has been generated. ScenarioId: {data.Id}, Count: {data.Items.Count}"));

                using (var innerScope = _repositoryFactory.BeginRepositoryScope())
                {
                    var failuresRepository = innerScope.CreateRepository<IFailuresRepository>();
                    failuresRepository.Add(data);
                    failuresRepository.SaveChanges();
                }

                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0,
                    0, $"Failures has been saved to the db. ScenarioId: {data.Id}"));
            }
            catch (Exception e)
            {
                _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForException(0, 0, "Error while processing failures", e));
                throw;
            }
        }
    }
}
