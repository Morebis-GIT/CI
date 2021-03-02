using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class TaskInstanceDomainModelHandler : IDomainModelHandler<TaskInstance>
    {
        private readonly ITaskInstanceRepository _taskInstanceRepository;
        private readonly ISqlServerMasterDbContext _dbContext;

        public TaskInstanceDomainModelHandler(ITaskInstanceRepository taskInstanceRepository, ISqlServerMasterDbContext dbContext)
        {
            _taskInstanceRepository = taskInstanceRepository;
            _dbContext = dbContext;
        }

        public TaskInstance Add(TaskInstance model)
        {
            _taskInstanceRepository.Add(model);
            return model;
        }

        public void AddRange(params TaskInstance[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<MasterEntities.TaskInstance>().Count();

        public void DeleteAll() => _dbContext.Truncate<MasterEntities.TaskInstance>();

        public IEnumerable<TaskInstance> GetAll() => _taskInstanceRepository.GetAll();
    }
}
