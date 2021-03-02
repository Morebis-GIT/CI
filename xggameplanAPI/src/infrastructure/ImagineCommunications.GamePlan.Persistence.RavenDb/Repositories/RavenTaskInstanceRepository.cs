using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenTaskInstanceRepository : ITaskInstanceRepository
    {
        private readonly IRavenMasterDbContext _dbContext;

        public RavenTaskInstanceRepository(IRavenMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TaskInstance Get(Guid id) =>
            _dbContext.Find<TaskInstance>(id);

        public List<TaskInstance> GetAll() =>
            _dbContext.Query<TaskInstance>().ToList();

        public void Add(TaskInstance taskInstance) =>
            _dbContext.Add(taskInstance);

        public void Update(TaskInstance taskInstance) =>
         _dbContext.Update(taskInstance);

        public void Remove(Guid id)
        {
            var entity = _dbContext.Find<TaskInstance>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();
    }
}
