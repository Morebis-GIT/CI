using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAnalysisGroupRepository : IAnalysisGroupRepository
    {
        private const int MaxClauseCount = 1000;
        private readonly IDocumentSession _session;

        public RavenAnalysisGroupRepository(IDocumentSession session) => _session = session;

        public void Add(AnalysisGroup analysisGroup)
        {
            lock (_session)
            {
                analysisGroup.IsDeleted = false;
                analysisGroup.DateCreated = DateTime.UtcNow;
                analysisGroup.DateModified = DateTime.UtcNow;
                _session.Store(analysisGroup);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = _session.Load<AnalysisGroup>(id);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public IEnumerable<AnalysisGroup> GetAll() =>
            _session.GetAll<AnalysisGroup>(x => !x.IsDeleted, AnalysisGroups_Default.DefaultIndexName, false);

        public AnalysisGroup Get(int id) =>
            _session.Query<AnalysisGroup>(AnalysisGroups_Default.DefaultIndexName).FirstOrDefault(x => !x.IsDeleted && x.Id == id);

        public AnalysisGroup GetIncludingSoftDeleted(int id) =>
            _session.Query<AnalysisGroup>(AnalysisGroups_Default.DefaultIndexName).FirstOrDefault(x => x.Id == id);

        public IEnumerable<AnalysisGroupNameModel> GetByIds(IEnumerable<int> ids, bool onlyActive = false)
        {
            var distinctIds = ids.Distinct().ToList();
            var result = new List<AnalysisGroup>();

            for (int i = 0, page = 0; i < distinctIds.Count; i += MaxClauseCount, page++)
            {
                var batch = distinctIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_session.GetAll<AnalysisGroup>(x => x.Id.In(batch)));
            }

            return result.Select(x => new AnalysisGroupNameModel
            {
                Id = x.Id,
                Name = x.Name,
                IsDeleted = x.IsDeleted
            }).Where(x => !onlyActive || !x.IsDeleted);
        }

        public AnalysisGroup GetByName(string name) =>
            _session.Query<AnalysisGroup>(AnalysisGroups_Default.DefaultIndexName).FirstOrDefault(x => !x.IsDeleted && x.Name == name);

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(AnalysisGroup analysisGroup)
        {
            lock (_session)
            {
                analysisGroup.DateModified = DateTime.UtcNow;
                _session.Store(analysisGroup);
            }
        }
    }
}
