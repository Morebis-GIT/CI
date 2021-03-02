using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Services
{
    public class ResultsFileTenantDbStorage : IResultsFileStorage
    {
        private readonly ISqlServerTenantDbContext _dbContext;

        protected static byte[] StreamToByteArray(Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Result file stream can't be read.");
            }

            var readLength = stream.Length - stream.Position;
            if (readLength == 0)
            {
                return Array.Empty<byte>();
            }

            var byteArray = new byte[readLength];
            var offset = 0;
            while (true)
            {
                var readBytes = stream.Read(byteArray, offset, (int) readLength);
                if (readBytes == readLength || readBytes == 0)
                {
                    break;
                }

                offset = readBytes;
                readLength -= readBytes;
            }

            return byteArray;
        }

        public ResultsFileTenantDbStorage(ISqlServerTenantDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Insert(Guid scenarioId, string fileId, Stream stream, bool compress)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            byte[] content;
            if (compress)
            {
                using (var compressedStream = CompressGzip(stream))
                {
                    content = StreamToByteArray(compressedStream);
                }
            }
            else
            {
                content = StreamToByteArray(stream);
            }

            _dbContext.Add(new ResultsFile
            {
                ScenarioId = scenarioId,
                FileId = fileId,
                IsCompressed = compress,
                FileContent = content
            });
        }

        public Stream Get(Guid scenarioId, string fileId, bool compressed)
        {
            var entity = _dbContext.Query<ResultsFile>()
                .FirstOrDefault(x => x.ScenarioId == scenarioId && x.FileId == fileId);
            if (entity == null)
            {
                throw new InvalidOperationException(
                    $"The result file requested (ScenarioId='{scenarioId}', FileId='{fileId}') does not exists on the database");
            }

            var contentStream = new MemoryStream(entity.FileContent);
            var needToDispose = true;
            try
            {
                if (compressed == entity.IsCompressed)
                {
                    needToDispose = false;
                    return contentStream;
                }

                if (compressed && !entity.IsCompressed)
                {
                    return CompressGzip(contentStream);
                }

                if (!compressed && entity.IsCompressed)
                {
                    return DecompressGzip(contentStream);
                }

                throw new InvalidOperationException("Getting result file error.");
            }
            finally
            {
                if (needToDispose)
                {
                    contentStream.Dispose();
                }
            }
        }

        public bool Delete(Guid scenarioId, string fileId)
        {
            var entity = _dbContext.Query<ResultsFile>()
                .FirstOrDefault(x => x.ScenarioId == scenarioId && x.FileId == fileId);
            if (entity != null)
            {
                _dbContext.Remove(entity);
                return true;
            }

            return false;
        }

        public bool Exists(Guid scenarioId, string fileId)
        {
            return _dbContext.Query<ResultsFile>()
                .Any(x => x.ScenarioId == scenarioId && x.FileId == fileId);
        }

        public void Clear()
        {
            _dbContext.Truncate<ResultsFile>();
        }

        public void Flush()
        {
            _dbContext.SaveChanges();
        }

        public int Count => _dbContext.Query<ResultsFile>().Count();

        /// <summary>
        /// Compress the specified stream
        /// </summary>
        protected Stream CompressGzip(Stream inputStream)
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
        protected Stream DecompressGzip(Stream inputStream)
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
