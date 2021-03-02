using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothFailureMessages
{
    public interface ISmoothFailureMessageRepository
    {
        /// <summary>
        /// Gets all failure types
        /// </summary>
        /// <returns></returns>
        IEnumerable<SmoothFailureMessage> GetAll();

        /// <summary>
        /// Adds failure types
        /// </summary>
        /// <param name="items"></param>
        void Add(IEnumerable<SmoothFailureMessage> items);
        void Truncate();
        void SaveChanges();
    }
}
