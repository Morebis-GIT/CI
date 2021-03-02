using System.IO;
using System.IO.Compression;

namespace xggameplan.common.Utilities
{
    public static class DeflateStreamCompression
    {
        /// <summary>
        /// Compresses byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] CompressBytes(byte[] data)
        {
            var output = new MemoryStream();
            using (var stream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                stream.Write(data, 0, data.Length);
            }

            return output.ToArray();
        }

        /// <summary>
        /// Decompresses byte array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] DecompressBytes(byte[] data)
        {
            var output = new MemoryStream();

            using (var input = new MemoryStream(data))
            using (var stream = new DeflateStream(input, CompressionMode.Decompress))
            {
                stream.CopyTo(output);
            }

            return output.ToArray();
        }
    }
}
