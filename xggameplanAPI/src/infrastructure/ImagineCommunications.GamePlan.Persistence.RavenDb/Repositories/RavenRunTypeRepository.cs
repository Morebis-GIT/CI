using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRunTypeRepository : IRunTypeRepository
    {
        private readonly IDocumentSession _session;

        public RavenRunTypeRepository(IDocumentSession session)
            => _session = session;

        public void Add(RunType runType)
        {
            lock (_session)
            {
                _session.Store(runType);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var runType = _session.Load<RunType>(id);

                if (runType is null)
                {
                    return;
                }

                _session.Delete(runType);
            }
        }

        public RunType FindByName(string name)
        {
            var runType = _session.Query<RunType>()
                .FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            var allAnalysisGroups = _session.GetAll<AnalysisGroup>();

            AddAnalysisGroupName(runType, allAnalysisGroups);

            return runType;
        }

        public RunType Get(int id)
        {
            var runType = _session.Load<RunType>(id);
            var allAnalysisGroups = _session.GetAll<AnalysisGroup>();

            AddAnalysisGroupName(runType, allAnalysisGroups);

            return runType;
        }

        public IEnumerable<RunType> GetByIds(IEnumerable<int> ids)
        {
            var runTypes = _session.Query<RunType>().Where(x => ids.Contains(x.Id)).ToList();
            var allAnalysisGroups = _session.GetAll<AnalysisGroup>();

            runTypes.ForEach(x => AddAnalysisGroupName(x, allAnalysisGroups));

            return runTypes;
        }

        public IEnumerable<RunType> GetAll()
        {
            var allRunTypes = _session.GetAll<RunType>();
            var allAnalysisGroups = _session.GetAll<AnalysisGroup>();

            foreach (var runType in allRunTypes)
            {
                AddAnalysisGroupName(runType, allAnalysisGroups);
            }

            return allRunTypes;
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(RunType runType)
        {
            lock (_session)
            {
                _session.Store(runType);
            }
        }

        private void AddAnalysisGroupName(RunType runType, IEnumerable<AnalysisGroup> analysisGroups)
        {
            if (runType is null)
            {
                return;
            }

            if (runType.RunTypeAnalysisGroups != null && runType.RunTypeAnalysisGroups.Any() && analysisGroups.Any())
            {
                runType.RunTypeAnalysisGroups
                    .ForEach(o => o.AnalysisGroupName = analysisGroups.SingleOrDefault(x => x.Id == o.AnalysisGroupId)?.Name);
            }
        }
    }
}
