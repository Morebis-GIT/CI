using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace xggameplan.Extensions
{
    /// <summary>
    /// Zip Serialization
    /// </summary>
    public static class ZipConversion
    {
        /// <summary>
        /// Combine all file memory stream and convert into Zip memory stream
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataToSerialize"></param>
        /// <returns></returns>
        public static MemoryStream SerializeIntoZip<T>(this Dictionary<string, T> dataToSerialize)
        {
            if (dataToSerialize == null)
            {
                throw new ArgumentNullException();
            }

            var outStream = new MemoryStream();
            try
            {
                using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                {
                    foreach (var data in dataToSerialize)
                    {
                        var fileInArchive = archive.CreateEntry(data.Key, CompressionLevel.Optimal);
                        using (var entryStream = fileInArchive.Open())
                        {
                            using (var fileToCompressStream = data.Value.Serialize()) // Calling existing file stream method
                            {
                                entryStream.Flush();
                                _ = fileToCompressStream.Seek(0, SeekOrigin.Begin);
                                fileToCompressStream.CopyTo(entryStream);
                                fileToCompressStream.Flush();
                            }
                        }
                    }
                }

                _ = outStream.Seek(0, SeekOrigin.Begin);

                return outStream;
            }
            catch
            {
                throw;
            }
        }

        public static void DeserializeFromZip(string zipFile, string outputFolder)
        {
            ZipFile.ExtractToDirectory(zipFile, outputFolder);
        }
    }
}
