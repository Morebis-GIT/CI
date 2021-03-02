using System.Collections.Generic;
using System.Linq;
using xggameplan.AuditEvents;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class MSTeamsAuditEventSettingsAdapter
        : RepositoryTestAdapter<MSTeamsAuditEventSettings, IMSTeamsAuditEventSettingsRepository, int>
    {
        public MSTeamsAuditEventSettingsAdapter(
            IScenarioDbContext dbContext,
            IMSTeamsAuditEventSettingsRepository repository
            ) : base(dbContext, repository) { }

        protected override MSTeamsAuditEventSettings Add(
            MSTeamsAuditEventSettings model
        )
        {
            Repository.Insert(new List<MSTeamsAuditEventSettings> {model});
            return model;
        }

        protected override IEnumerable<MSTeamsAuditEventSettings> AddRange(
            params MSTeamsAuditEventSettings[] models
        )
        {
            Repository.Insert(models.ToList());
            return models;
        }

        protected override int Count() =>
            Repository.GetAll().Count;

        protected override void Delete(int id) =>
            Repository.DeleteByID(id);

        protected override IEnumerable<MSTeamsAuditEventSettings> GetAll() =>
            Repository.GetAll();

        protected override MSTeamsAuditEventSettings GetById(int id) =>
            Repository.GetAll()
                .First(mst => mst.EventTypeId == id);

        protected override void Truncate() =>
            Repository.DeleteAll();

        protected override MSTeamsAuditEventSettings Update(
            MSTeamsAuditEventSettings model
        )
        {
            Repository.Update(model);
            return model;
        }
    }
}
