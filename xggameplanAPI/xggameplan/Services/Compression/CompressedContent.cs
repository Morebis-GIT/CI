using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.IO.Compression;
using System.Threading.Tasks;

namespace xggameplan.Services.Compression
{
    /// <summary>
    /// Handles content compression using gzip/deflate
    /// </summary>
    public class CompressedContent : HttpContent
    {
        private readonly HttpContent _content;
        private readonly string _encodingType;

        private const string ContentLengthHeaderName = "Content-Length";
        private const string GzipContentEncoding = "gzip";
        private const string DeflateContentEncoding = "deflate";

        /// <summary>
        /// Initializes an instance of <see cref="CompressedContent"/>.
        /// </summary>
        /// <param name="content">A source response content.</param>
        /// <param name="encodingType">An encoding type.</param>
        public CompressedContent(HttpContent content, string encodingType)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _encodingType = ValidateEncodingType(encodingType);

            ConfigureHeaders();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Stream compressedStream;

            switch (_encodingType)
            {
                case GzipContentEncoding:
                    compressedStream = new GZipStream(stream, CompressionMode.Compress, true);
                    break;
                case DeflateContentEncoding:
                    compressedStream = new DeflateStream(stream, CompressionMode.Compress, true);
                    break;
                default:
                    throw new ArgumentException($"Encoding {_encodingType} is not supported");
            }

            return _content.CopyToAsync(compressedStream).ContinueWith(tsk =>
            {
                compressedStream?.Dispose();
            });
        }

        private void ConfigureHeaders()
        {
            foreach (var header in _content.Headers)
            {
                if (header.Key.Equals(ContentLengthHeaderName))
                {
                    continue;
                }

                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            Headers.ContentEncoding.Add(_encodingType);
        }

        private static string ValidateEncodingType(string encodingType)
        {
            if (string.IsNullOrEmpty(encodingType))
            {
                throw new ArgumentNullException(nameof(encodingType));
            }

            if (Array.IndexOf(new[] { GzipContentEncoding, DeflateContentEncoding }, encodingType) == -1)
            {
                throw new ArgumentException($"Encoding {encodingType} is not supported");
            }

            return encodingType.ToLowerInvariant();
        }

        protected override void Dispose(bool disposing)
        {
            _content?.Dispose();
        }
    }
}
