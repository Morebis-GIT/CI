using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ScheduleDomainModelHandler : IDomainModelHandler<Schedule>
    {
        private readonly IScheduleRepository _repository;

        public ScheduleDomainModelHandler(IScheduleRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Schedule Add(Schedule model)
        {
            _repository.Add(model);
            return model;
        }

        public void AddRange(params Schedule[] models)
        {
            foreach(var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _repository.CountAll;
        
        public void DeleteAll() => _repository.Truncate();

        public IEnumerable<Schedule> GetAll() => _repository.GetAll();
    }
}
