using System;
using System.IO;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ResultsFileRepository : IResultsFileRepository
    {
        private const bool Compression = true;
        private readonly IResultsFileStorage _resultFileStorage;

        public ResultsFileRepository(IResultsFileStorage resultFileStorage)
        {
            _resultFileStorage = resultFileStorage;
        }

        public void Insert(Guid scenarioId, string fileId, string localFolder)
        {
            var localFile = Path.Combine(localFolder, GetFileName(fileId, false));
            if (!System.IO.File.Exists(localFile))
            {
                throw new FileNotFoundException($"Results file {localFile} does not exist");
            }
            using (var stream = File.OpenRead(localFile))
            {
                _resultFileStorage.Insert(scenarioId, fileId, stream, Compression);
                _resultFileStorage.Flush();
            }
        }

        public void Get(Guid scenarioId, string fileId, bool compressed, string localFolder)
        {
            var localFile = Path.Combine(localFolder, GetFileName(fileId, compressed));
            if (File.Exists(localFile))
            {
                File.Delete(localFile);
            }

            using (var fileStream = File.Create(localFile))
            using (var outputStream = _resultFileStorage.Get(scenarioId, fileId, compressed))
            {
                outputStream.CopyTo(fileStream);
            }
        }

        public void Delete(Guid scenario, string fileId)
        {
            _resultFileStorage.Delete(scenario, fileId);
            _resultFileStorage.Flush();
        }

        public bool Exists(Guid scenarioId, string fileId)
        {
            return _resultFileStorage.Exists(scenarioId, fileId);
        }

        protected string GetFileName(string fileId, bool compressed) =>
            compressed ? $"{fileId}.zip" : fileId;
    }
}
