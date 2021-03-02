using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Channels
{
    /// <summary>
    /// Channels Repository
    /// </summary>
    public interface IChannelsRepository
    {
        /// <summary>
        /// get all Channel
        /// </summary>
        /// <returns></returns>
        IEnumerable<Channel> GetAll();

        /// <summary>
        /// Add channel
        /// </summary>
        /// <param name="channel"></param>
        void Add(Channel channel);

        /// <summary>
        /// get channel by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Channel GetById(int id);

        /// <summary>
        /// delete channel by id
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);
    }
}
