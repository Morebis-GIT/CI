using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace xggameplan.Services
{
    public class CompressionUtilities
    {
        public static Stream CompressGZip(Stream inputStream)
        {
            using (MemoryStream memory = new MemoryStream())
            using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
            {
                inputStream.CopyTo(gzip);
                return memory;
            }
        }

        public static void CompressGZipToFile(string inputFile, string outputFile)
        {
            FileInfo inputFileInfo = new FileInfo(inputFile);
            FileInfo gzipFileName = new FileInfo(outputFile);

            using (FileStream fileToBeZippedAsStream = inputFileInfo.OpenRead())
            using (FileStream gzipTargetAsStream = gzipFileName.Create())
            using (GZipStream gzipStream = new GZipStream(gzipTargetAsStream, CompressionMode.Compress))
            {
                fileToBeZippedAsStream.CopyTo(gzipStream);
            }
        }

        public static async Task CompressGZipToFileAsync(string inputFile, string outputFile)
        {
            FileInfo inputFileInfo = new FileInfo(inputFile);
            FileInfo gzipFileName = new FileInfo(outputFile);

            using (FileStream fileToBeZippedAsStream = inputFileInfo.OpenRead())
            using (FileStream gzipTargetAsStream = gzipFileName.Create())
            using (GZipStream gzipStream = new GZipStream(gzipTargetAsStream, CompressionMode.Compress))
            {
                await fileToBeZippedAsStream.CopyToAsync(gzipStream);
            }
        }

        /// <summary>
        /// Decompresses gzip stream
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static Stream DecompressGZIP(Stream inputStream)
        {
            using (Stream outputStream = new MemoryStream())
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
                outputStream.Seek(0, SeekOrigin.Begin);

                return outputStream;
            }
        }
    }
}
