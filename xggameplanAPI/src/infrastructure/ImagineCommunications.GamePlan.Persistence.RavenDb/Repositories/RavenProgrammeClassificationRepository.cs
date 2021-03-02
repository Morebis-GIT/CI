using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenProgrammeClassificationRepository : IProgrammeClassificationRepository
    {
        private readonly IDocumentSession _session;

        public RavenProgrammeClassificationRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(IEnumerable<ProgrammeClassification> items)
        {
            lock (_session)
            {
                using (_session.Advanced.DocumentStore.BulkInsert())
                {
                    items.ToList().ForEach(item => _session.Store(item));
                }
            }
        }

        public void Add(ProgrammeClassification item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        public ProgrammeClassification GetByCode(string code)
        {
            lock (_session)
            {
                var item = _session.Query<ProgrammeClassification>().FirstOrDefault(_ => _.Code == code);
                return item;
            }
        }

        public ProgrammeClassification GetById(int id)
        {
            lock (_session)
            {
                var item = _session.Query<ProgrammeClassification>().FirstOrDefault(_ => _.Uid == id);
                return item;
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = _session.Query<ProgrammeClassification>().FirstOrDefault(_ => _.Uid == id);
                _session.Delete(item);
            }
        }

        public IEnumerable<ProgrammeClassification> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<ProgrammeClassification>().ToList();
            }
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<ProgrammeClassification>();
                }
            }
        }

        public void Update(ProgrammeClassification programmeClassification)
        {
            lock (_session)
            {
                _session.Store(programmeClassification);
            }
        }

        public void Truncate() =>
            _session.TryDelete("Raven/DocumentsByEntityName", "ProgrammeClassifications");

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
