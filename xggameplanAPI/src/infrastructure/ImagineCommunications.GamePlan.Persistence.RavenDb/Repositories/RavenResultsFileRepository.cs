using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.ResultsFiles;
using Raven.Client.FileSystem;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    /// <summary>
    /// Repository for AutoBook results
    /// </summary>
    public class RavenResultsFileRepository : IResultsFileRepository
    {
        private readonly IAsyncFilesSession _session;
        private const bool _compression = true;

        public RavenResultsFileRepository(IAsyncFilesSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Inserts results file
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fileId"></param>
        /// <param name="localFolder"></param>
        public void Insert(Guid scenarioId, string fileId, string localFolder)
        {
            InsertTask(scenarioId, fileId, localFolder).Wait();
        }

        private async Task InsertTask(Guid scenarioId, string fileId, string localFolder)
        {
            var localFile = Path.Combine(localFolder, GetFileName(fileId, false));
            if (!System.IO.File.Exists(localFile))
            {
                throw new FileNotFoundException($"Results file {localFile} does not exist");
            }
            var uploadFilePath = GetUploadFilePath(scenarioId, fileId, _compression);
            using (var stream = File.OpenRead(localFile))
            using (var outputStream = CompressGZIP(stream))
            {
                _session.RegisterUpload(uploadFilePath, outputStream);
                await _session.SaveChangesAsync().ConfigureAwait(false); // actually upload the file
            }
        }

        public bool Exists(Guid scenarioId, string fileId)
        {
            var uploadFilePath = GetUploadFilePath(scenarioId, fileId, _compression);
            var task = _session.LoadFileAsync(uploadFilePath);
            task.Wait();
            if (task.Exception == null)
            {
                return (task.Result != null);
            }
            throw task.Exception;
        }

        /// <summary>
        /// Gets results file to local file
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fileId"></param>
        /// <param name="compressed"></param>
        /// <param name="localFolder"></param>
        public void Get(Guid scenarioId, string fileId, bool compressed, string localFolder)
        {
            GetTask(scenarioId, fileId, compressed, localFolder).Wait();
        }

        private async Task GetTask(Guid scenarioId, string fileId, bool compressed, string localFolder)
        {
            // Set name of local file
            var localFile = Path.Combine(localFolder, GetFileName(fileId, compressed));
            if (File.Exists(localFile))
            {
                File.Delete(localFile);
            }

            var uploadFilePath = GetUploadFilePath(scenarioId, fileId, _compression);
            using (var downloadedStream = await _session.DownloadAsync(uploadFilePath).ConfigureAwait(false))
            using (var fileStream = System.IO.File.Create(localFile))
            {
                if (compressed) // Leave compressed
                {
                    downloadedStream.CopyTo(fileStream);
                }
                else // Decompress
                {
                    using (var outputStream = DecompressGZIP(downloadedStream))
                    {
                        outputStream.CopyTo(fileStream);
                    }
                }
            }
        }

        private string GetFileName(string fileId, bool compressed) =>
            compressed ? $"{fileId}.zip" : fileId;

        private string GetUploadFilePath(Guid scenarioId, string fileId, bool compression) =>
            $"scenarioresults/{scenarioId}/{GetFileName(fileId, compression)}";

        /// <summary>
        /// Deletes results file
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="fileId"></param>
        public void Delete(Guid scenarioId, string fileId)
        {
            DeleteTask(scenarioId, fileId).Wait();
        }

        private async Task DeleteTask(Guid scenarioId, string fileId)
        {
            var uploadFilePath = GetUploadFilePath(scenarioId, fileId, _compression);
            _session.RegisterFileDeletion(uploadFilePath);
            await _session.SaveChangesAsync().ConfigureAwait(false); // actually upload the file
        }

        /// <summary>
        /// Compress the specified stream
        /// </summary>
        private Stream CompressGZIP(Stream inputStream)
        {
            var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress, true))
            {
                inputStream.CopyTo(gzipStream);
            }
            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }

        /// <summary>
        /// Decompresses gzip stream
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        private Stream DecompressGZIP(Stream inputStream)
        {
            var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
            {
                gzipStream.CopyTo(outputStream);
            }
            outputStream.Seek(0, SeekOrigin.Begin);
            return outputStream;
        }
    }
}
