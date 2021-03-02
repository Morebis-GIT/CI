using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Channels;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    /// <summary>
    /// Raven Channel Repository
    /// </summary>
    public class RavenChannelRepository : IChannelsRepository
    {
        private readonly IDocumentSession _session;

        /// <summary>
        /// Raven Channel Repository
        /// </summary>
        /// <param name="session"></param>
        public RavenChannelRepository(IDocumentSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Get all channels from the data store
        /// </summary>
        /// <returns>Returns a collection containing all channels.</returns>
        public IEnumerable<Channel> GetAll() =>
            _session.GetAll<Channel>();

        /// <summary>
        /// Add new entry in channels
        /// </summary>
        /// <param name="channel">Channel item</param>
        public void Add(Channel channel) =>
            _session.Store(channel);


        [Obsolete("Use the Get() method")]
        public Channel GetById(int id) => Get(id);

        /// <summary>
        /// Get channel by id
        /// </summary>
        /// <param name="id">Channel Id</param>
        /// <returns></returns>
        public Channel Get(int id) =>
            _session.Load<Channel>(id);

        /// <summary>
        /// Delete channel by id
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id) =>
            _session.Delete<Channel>(id);
    }
}
