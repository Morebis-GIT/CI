using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Maintenance.TaskInstances;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class TaskInstanceRepositoryAdapter : RepositoryTestAdapter<TaskInstance, ITaskInstanceRepository, Guid>
    {
        public TaskInstanceRepositoryAdapter(IScenarioDbContext dbContext, ITaskInstanceRepository repository)
            : base(dbContext, repository)
        {
        }

        protected override TaskInstance Add(TaskInstance model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<TaskInstance> AddRange(params TaskInstance[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(Guid id)
        {
            Repository.Remove(id);
        }

        protected override IEnumerable<TaskInstance> GetAll() =>
            Repository.GetAll();

        protected override TaskInstance GetById(Guid id) =>
            Repository.Get(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override TaskInstance Update(TaskInstance model)
        {
            Repository.Update(model);
            return model;
        }
    }
}

