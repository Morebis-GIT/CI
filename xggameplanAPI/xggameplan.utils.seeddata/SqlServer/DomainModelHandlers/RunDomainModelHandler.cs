using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using RunEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.Run;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class RunDomainModelHandler : IDomainModelHandler<Run>
    {
        private readonly IRunRepository _runRepository;
        private readonly ISqlServerDbContext _dbContext;

        public RunDomainModelHandler(IRunRepository runRepository,
            ISqlServerDbContext dbContext)
        {
            _runRepository =
                runRepository ?? throw new ArgumentNullException(nameof(runRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Run Add(Run model)
        {
            _runRepository.Add(model);
            return model;
        }

        public void AddRange(params Run[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<RunEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<RunEntity>();

        public IEnumerable<Run> GetAll() => _runRepository.GetAll();
    }
}
