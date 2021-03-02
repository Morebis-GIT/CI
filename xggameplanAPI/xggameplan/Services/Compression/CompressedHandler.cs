using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.IO.Compression;
using System.Threading.Tasks;

namespace xggameplan.Services.Compression
{
    /// <summary>
    /// Handles for compressed requests.
    /// </summary>
    internal class CompressedHandler : DelegatingHandler
    {
        private readonly bool _compressResponses;
        private readonly int _minLengthForCompressResponse;

        private const string ContentEncodingHeaderName = "Content-Encoding";
        private const string GzipContentEncoding = "gzip";
        private const string DeflateContentEncoding = "deflate";

        /// <summary>
        /// Initializes an instance of <see cref="CompressedHandler"/>.
        /// </summary>
        /// <param name="compressResponses">The flag represents is there enabled compressing of responses.</param>
        /// <param name="minLengthForCompressResponse">The minimum threshold value of response that enables compression.
        /// Minimal recommended threshold for gzip 150-1000.</param>
        public CompressedHandler(bool compressResponses = true, int minLengthForCompressResponse = 500)
        {
            _compressResponses = compressResponses;
            _minLengthForCompressResponse = minLengthForCompressResponse;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsRequestContentCompressed(request.Content, out var decompressFunction))
            {
                await DecompressRequestContentAsync(request, decompressFunction);
            }

            var response = await base.SendAsync(request, cancellationToken);

            await SetContentLengthHeaderAsync(response);

            if (IsCompressionNecessary(response, out var encodingType))
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.Now} - Compressing response");
                response.Content = new CompressedContent(response.Content, encodingType);
            }

            return response;
        }

        private bool IsCompressionNecessary(HttpResponseMessage response, out string encodingType)
        {
            encodingType = null;

            if (response?.Content == null)
            {
                return false;
            }

            var isResponseAlreadyCompressed = IsResponseAlreadyCompressed(response);
            if (!_compressResponses || isResponseAlreadyCompressed)
            {
                return false;
            }

            var contentHeaderValue = GetContentHeaderValue(response.Content, ContentEncodingHeaderName);
            if (!string.IsNullOrEmpty(contentHeaderValue))
            {
                return false;
            }

            var contentLength = response.Content.Headers.ContentLength.GetValueOrDefault(-1);
            if (contentLength < _minLengthForCompressResponse)
            {
                return false;
            }

            encodingType = response.RequestMessage?.Headers?.AcceptEncoding?.FirstOrDefault()?.Value;
            return !string.IsNullOrEmpty(encodingType);
        }

        private static async Task SetContentLengthHeaderAsync(HttpResponseMessage response)
        {
            if (response?.Content == null)
            {
                return;
            }
            var contentLength = response.Content.Headers.ContentLength ?? (await response.Content.ReadAsByteArrayAsync()).Length;
            response.Content.Headers.ContentLength = contentLength;
        }

        private static bool IsResponseAlreadyCompressed(HttpResponseMessage response)
        {
            const string isCompressedContentHeaderName = "IsCompressedContent";

            if (!response.Content.Headers.TryGetValues(isCompressedContentHeaderName, out var values))
            {
                return false;
            }

            return bool.TryParse(values?.FirstOrDefault(), out var isResponseAlreadyCompressed)
                    && isResponseAlreadyCompressed;
        }

        private static async Task DecompressRequestContentAsync(HttpRequestMessage request, Func<Stream, Stream> decompressFunction)
        {
            var requestContentStream = await request.Content.ReadAsStreamAsync();
            var outputStream = decompressFunction(requestContentStream);

            var origContent = request.Content;

            request.Content = new StreamContent(outputStream);

            foreach (var header in origContent.Headers)
            {
                request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        private static bool IsRequestContentCompressed(HttpContent requestContent, out Func<Stream, Stream> decompressFunction)
        {
            var contentEncoding = GetContentHeaderValue(requestContent, ContentEncodingHeaderName);
            decompressFunction = GetDecompressFunction(contentEncoding);

            return requestContent?.Headers.ContentType != null && decompressFunction != null;
        }

        private static string GetContentHeaderValue(HttpContent content, string headerName)
        {
            if (content?.Headers == null)
            {
                return null;
            }

            foreach (var header in content.Headers)
            {
                if (header.Key.Equals(headerName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return content.Headers.GetValues(header.Key).First();
                }
            }

            return null;
        }

        private static Func<Stream, Stream> GetDecompressFunction(string contentEncoding)
        {
            if (string.IsNullOrEmpty(contentEncoding))
            {
                return null;
            }

            if (contentEncoding.Contains(GzipContentEncoding))
            {
                return DecompressGzip;
            }

            if (contentEncoding.Contains(DeflateContentEncoding))
            {
                return DecompressDeflate;
            }

            return null;
        }

        private static Stream DecompressGzip(Stream inputCompressedStream)
        {
            Stream outputStream = new MemoryStream();

            using (var gzipStream = new GZipStream(inputCompressedStream, CompressionMode.Decompress))
            {
                gzipStream.CopyTo(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }

        private static Stream DecompressDeflate(Stream inputCompressedStream)
        {
            Stream outputStream = new MemoryStream();

            using (var deflateStream = new DeflateStream(inputCompressedStream, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(outputStream);
            }

            outputStream.Seek(0, SeekOrigin.Begin);

            return outputStream;
        }
    }
}
