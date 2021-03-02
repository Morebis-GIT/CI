using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances
{
    public interface ITaskInstanceRepository
    {
        void Add(TaskInstance taskInstance);

        TaskInstance Get(Guid id);

        List<TaskInstance> GetAll();

        void Update(TaskInstance taskInstance);

        void Remove(Guid id);

        void SaveChanges();
    }
}
