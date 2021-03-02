using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using ImagineCommunications.Intelligence.Test.Api.Models;

namespace ImagineCommunications.Intelligence.Test.Api.Services
{
    public interface ILogFileExportService
    {
        string MediaType { get; }
        Task<FileModel> GetLogFileAsync(DateTime date);
    }

    public class LogFileExportService : ILogFileExportService
    {
        public string MediaType => "application/octet-stream";

        private readonly string _filePath;
        public LogFileExportService(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            _filePath = filePath;
        }

        public async Task<FileModel> GetLogFileAsync(DateTime date)
        {
            var compressedFilePath = string.Empty;
            try
            {
                var formattedDate = date.ToString("yyyyMMdd");

                var fileFormat = ".txt";
                var filePath = _filePath.EndsWith(fileFormat)
                    ? _filePath.Replace(fileFormat, "")
                    : _filePath;

                filePath = $"{filePath}{formattedDate}{fileFormat}";

                if (!File.Exists(filePath))
                {
                    return null;
                }

                var filename = Guid.NewGuid().ToString();
                compressedFilePath = Path.Combine(Path.GetDirectoryName(filePath), $"{filename}.zip");

                await CompressGZipToFileAsync(filePath, compressedFilePath).ConfigureAwait(false);

                return new FileModel()
                {
                    Name = Path.GetFileName(filePath),
                    Content = File.ReadAllBytes(compressedFilePath),
                };
            }
            finally
            {
                if (!string.IsNullOrEmpty(compressedFilePath) && File.Exists(compressedFilePath))
                {
                    File.Delete(compressedFilePath);
                }
            }
        }

        private static async Task CompressGZipToFileAsync(string inputFile, string outputFile)
        {
            FileInfo inputFileInfo = new FileInfo(inputFile);
            FileInfo gzipFileName = new FileInfo(outputFile);

            using (FileStream fileToBeZippedAsStream = inputFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (FileStream gzipTargetAsStream = gzipFileName.Create())
            using (GZipStream gzipStream = new GZipStream(gzipTargetAsStream, CompressionMode.Compress))
            {
                await fileToBeZippedAsStream.CopyToAsync(gzipStream);
            }
        }
    }
}
