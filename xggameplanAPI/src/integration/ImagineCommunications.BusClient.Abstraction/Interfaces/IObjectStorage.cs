using System;
using System.IO;
using System.Threading.Tasks;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    /// <summary>
    /// The object storage.
    /// </summary>
    /// <seealso cref="IBigMessage"/>
    public interface IObjectStorage : IDisposable              
    {
        /// <summary>
        /// Stores the stream of object.
        /// </summary>
        /// <param name="stream">The stream of object.</param>
        /// <returns>The URI to stored object.</returns>
        Task<Uri> StoreAsync(Stream stream);

        /// <summary>
        /// Gets the stream of object at the specified URI.
        /// </summary>
        /// <param name="uri">The URI of the object.</param>
        /// <returns>The stream of object.</returns>
        Task<Stream> GetAsync(Uri uri);

        /// <summary>
        /// Deletes the object at the specified URI.
        /// </summary>
        /// <param name="uri">The URI of object.</param>
        Task DeleteAsync(Uri uri);
    }
}
