using System;
using System.IO;
using System.IO.Compression;

namespace xggameplan.common.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Compress the specified stream
        /// </summary>
        public static Stream Compress(this Stream inputStream)
        {
            var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionMode.Compress, true))
            {
                inputStream.CopyTo(gzipStream);
            }

            _ = outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        /// <summary>
        /// Decompresses gzip stream
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static Stream Decompress(this Stream inputStream)
        {
            var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress, true))
            {
                gzipStream.CopyTo(outputStream);
            }

            _ = outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new InvalidOperationException("Result file stream can't be read.");
            }

            var position = stream.CanSeek ? stream.Position : 0;

            var readLength = stream.Length - position;
            if (readLength == 0)
            {
                return Array.Empty<byte>();
            }

            var byteArray = new byte[stream.Length];
            var offset = 0;
            while (true)
            {
                var readBytes = stream.Read(byteArray, offset, (int)readLength);
                if (readBytes == readLength || readBytes == 0)
                {
                    break;
                }

                offset = readBytes;
                readLength -= readBytes;
            }

            return byteArray;
        }
    }
}
