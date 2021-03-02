using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace xggameplan.cloudaccess.Interfaces
{
    /// <summary>
    /// Exposes infrastructure to work with S3 bucket files asynchronously.
    /// </summary>
    public interface ICloudStorageV2 : IDisposable
    {
        Task<string> GetPreSignedUrlAsync(string fullFileName);

        Task<string> DownloadAsync(string fullFileName, string localFileName,
            CancellationToken cancellationToken = default);

        Task UploadAsync(string localFileName, string fullFileName,
            CancellationToken cancellationToken = default);

        Task UploadAsync(Stream stream, string fullFileName, CancellationToken cancellationToken = default);

        Task<bool> FileExistsAsync(string fullFileName, CancellationToken cancellationToken = default);

        Task DeleteAsync(string fullFileName, CancellationToken cancellationToken = default);

        Task DeleteAsync(IReadOnlyCollection<string> fullFileNames, CancellationToken cancellationToken = default);

        Task DeleteByPrefixAsync(string filePrefix, CancellationToken cancellationToken = default);

        Task DeleteByPrefixesAsync(IReadOnlyCollection<string> filePrefixes,
            CancellationToken cancellationToken = default);
    }
}
