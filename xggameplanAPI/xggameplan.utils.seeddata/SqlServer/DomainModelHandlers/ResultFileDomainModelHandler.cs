using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.utils.seeddata.Migration.RavenToSql;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ResultFileDomainModelHandler : IDomainModelHandler<ResultFileMigrationDocumentHandler.ResultFile>
    {
        private readonly IResultsFileStorage _resultFileStorage;

        public ResultFileDomainModelHandler(IResultsFileStorage resultFileStorage)
        {
            _resultFileStorage = resultFileStorage ?? throw new ArgumentNullException(nameof(resultFileStorage));
        }

        public ResultFileMigrationDocumentHandler.ResultFile Add(ResultFileMigrationDocumentHandler.ResultFile model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            using (var stream = new MemoryStream(model.Content))
            {
                _resultFileStorage.Insert(model.ScenarioId, model.FileId, stream, model.IsCompressed);
            }
            return model;
        }

        public void AddRange(params ResultFileMigrationDocumentHandler.ResultFile[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _resultFileStorage.Count;

        public void DeleteAll() => _resultFileStorage.Clear();

        public IEnumerable<ResultFileMigrationDocumentHandler.ResultFile> GetAll() =>
            throw new NotSupportedException();
    }
}
