using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.OutputFiles;
using ImagineCommunications.GamePlan.Domain.OutputFiles.Objects;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    /// <summary>
    /// Raven repository for output files
    /// </summary>
    public class RavenOutputFileRepository : IOutputFileRepository
    {
        private readonly IDocumentSession _session;

        public RavenOutputFileRepository(IDocumentSession session)
        {
            _session = session;
        }

        public OutputFile Find(string id)
        {
            return GetAll().Where(item => item.FileId == id).FirstOrDefault();
        }

        public List<OutputFile> GetAll()
        {
            return _session.Query<OutputFile>().Take(int.MaxValue).ToList();
        }

        public void Insert(OutputFile outputFile)
        {
            _session.Store(outputFile);
        }
    }
}
