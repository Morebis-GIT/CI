using System.IO;
using System.Threading.Tasks;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    /// <summary>
    /// The big message service.
    /// </summary>
    /// <seealso cref="IBigMessage"/>
    public interface IBigMessageService
    {
        /// <summary>
        /// Stores the given message as big message.
        /// </summary>
        /// <typeparam name="T">The type of message.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>The big message.</returns>
        Task<IBigMessage> StoreMessageAsync<T>(Stream message) where T : IEvent;

        /// <summary>
        /// Serializes the message.
        /// </summary>
        /// <typeparam name="T">The type of message.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns>The stream of message.</returns>
        Stream SerializeMessage<T>(T message) where T : IEvent;

        /// <summary>
        /// Gets the message from big message.
        /// </summary>
        /// <param name="bigMessage">The big message.</param>
        /// <returns>The message.</returns>
        Task<object> GetMessageAsync(IBigMessage bigMessage);
    }
}
