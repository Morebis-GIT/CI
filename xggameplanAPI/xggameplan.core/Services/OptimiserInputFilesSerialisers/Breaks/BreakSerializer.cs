using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using Microsoft.Extensions.Configuration;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Breaks
{
    /// <summary>
    /// Breaks
    /// </summary>
    public class BreakSerializer : BreakSerializerBase
    {
        private readonly IScheduleRepository _scheduleRepository;

        public BreakSerializer(
            IAuditEventRepository auditEventRepository,
            IFeatureManager featureManager,
            IScheduleRepository scheduleRepository,
            IRepositoryFactory repositoryFactory,
            IConfiguration applicationConfiguration,
            IMapper mapper,
            IClock clock) :
            base(auditEventRepository, featureManager, repositoryFactory, applicationConfiguration,
                mapper, clock)
        {
            _scheduleRepository = scheduleRepository;
        }

        protected override IReadOnlyCollection<BreakWithProgramme> GetBreaksWithProgrammes(Run run,
            IReadOnlyCollection<SalesArea> salesAreas)
        {
            return _scheduleRepository.GetBreakModels(
                    salesAreas.Select(s => s.Name).ToList(),
                    run.StartDate.Add(run.StartTime),
                    DateHelper.ConvertBroadcastToStandard(run.EndDate, run.EndTime),
                    ExcludeBreakType);
        }
    }
}
