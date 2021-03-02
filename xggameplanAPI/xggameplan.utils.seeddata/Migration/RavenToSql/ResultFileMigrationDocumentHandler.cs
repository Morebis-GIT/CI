using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Features.Indexed;
using Raven.Client.FileSystem;
using Serilog;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class ResultFileMigrationDocumentHandler
        : RavenToSqlMigrationDocumentHandler<ResultFileMigrationDocumentHandler.ResultFile>
    {
        public class ResultFile
        {
            public Guid ScenarioId { get; set; }
            public string FileId { get; set; }
            public bool IsCompressed { get; set; }
            public byte[] Content { get; set; }
        }

        public ResultFileMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex,
            ILogger logger) : base(containerIndex, logger)
        {
        }

        protected override IEnumerable<ResultFile> GetSourceDocuments()
        {
            var fileSession = SourceContainer.Resolve<IAsyncFilesSession>();
            const int size = 1024;
            var page = 0;

            var query = fileSession.Query().OnDirectory("scenarioresults", true).Take(size);
            FilesQueryStatistics stats = null;

            do
            {
                var source = query.Skip(page * size);
                source = page == 0 ? source.Statistics(out stats) : source;
                foreach (var item in source.ToListAsync().Result)
                {
                    var scenarioDir = Path.GetFileName(item.Directory);
                    if (scenarioDir != null && Guid.TryParse(scenarioDir, out var scenarioId))
                    {
                        using (var stream = fileSession.DownloadAsync(item).Result)
                        using(var contentStream = new MemoryStream())
                        {
                            stream.CopyTo(contentStream);
                            yield return new ResultFile
                            {
                                ScenarioId = scenarioId,
                                FileId = Path.GetFileNameWithoutExtension(item.Name),
                                IsCompressed =
                                    item.Extension?.Equals(".zip", StringComparison.InvariantCultureIgnoreCase) ??
                                    false,
                                Content = contentStream.ToArray()
                            };
                        }
                    }
                }
                page++;
            } while (page * size <= (stats?.TotalResults ?? 0));
        }
    }
}
