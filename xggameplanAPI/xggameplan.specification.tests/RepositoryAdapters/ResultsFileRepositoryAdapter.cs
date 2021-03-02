using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using xggameplan.specification.tests.Coordinators;
using xggameplan.specification.tests.Infrastructure.RepositoryMethod;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ResultsFileRepositoryAdapter : NoCrudRepositoryTestAdapter<dynamic, IResultsFileRepository, Guid>
    {
        protected class ExistsResult
        {
            public bool Result { get; set; }
        }

        private readonly ResultsFileCoordinator _resultsFileCoordinator;

        public ResultsFileRepositoryAdapter(IScenarioDbContext dbContext, IResultsFileRepository repository,
            ResultsFileCoordinator resultsFileCoordinator) : base(dbContext, repository)
        {
            _resultsFileCoordinator = resultsFileCoordinator;
        }

        protected override dynamic GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<dynamic> GetAll()
        {
            throw new NotImplementedException();
        }

        [RepositoryMethod]
        protected CallMethodResult Insert(Guid scenarioId, string fileId)
        {
            _resultsFileCoordinator.InsertResultFile(scenarioId, fileId);
            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult Get(Guid scenarioId, string fileId, bool compressed)
        {
            _resultsFileCoordinator.GetResultFile(scenarioId, fileId, compressed);
            return CallMethodResult.CreateHandled();
        }

        [RepositoryMethod]
        protected CallMethodResult Exists(Guid scenarioId, string fileId)
        {
            var res = Repository.Exists(scenarioId, fileId);
            AssignTestContextSingleResult(new ExistsResult
            {
                Result = res
            });
            return CallMethodResult.CreateHandled();
        }
    }
}
