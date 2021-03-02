using System.Collections.Generic;
using xggameplan.Model;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for communication with AutoBooks API (Provisioning API).
    /// </summary>
    /// <remarks>
    /// Type parameter <see cref="TAutoBookDetails"/> is a class for details about AutoBook.
    /// Type parameter <see cref="TAutoBookCreateDetails"/> is a class for creating AutoBook.
    /// </remarks>
    public interface IAutoBooksAPI<TAutoBookDetails, TAutoBookCreateDetails>
    {
        /// <summary>
        /// Creates an AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        TAutoBookDetails Create(TAutoBookCreateDetails autoBook);

        /// <summary>
        /// Returns an AutoBook instance
        /// </summary>
        /// <param name="autoBookId"></param>
        /// <returns></returns>
        TAutoBookDetails Get(string autoBookId);

        /// <summary>
        /// Returns list of AutoBook instances
        /// </summary>
        /// <returns></returns>
        List<TAutoBookDetails> GetAll();

        /// <summary>
        /// Updates AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        void Update(TAutoBookDetails autoBook);

        /// <summary>
        /// Deletes AutoBook
        /// </summary>
        /// <param name="autoBookId"></param>
        void Delete(string autoBookId);

        /// <summary>
        /// Restarts AutoBook
        /// </summary>
        /// <param name="autoBookId"></param>
        void Restart(string autoBookId);

        /// <summary>
        /// Requests an autobookRequest model
        /// </summary>
        /// <param name="autoBookRequest"></param>
        string AutoBookRequestRun(AutoBookRequestModel autoBookRequest);
    }
}
