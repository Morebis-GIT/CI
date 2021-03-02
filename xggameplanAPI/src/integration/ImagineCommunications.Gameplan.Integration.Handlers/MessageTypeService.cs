using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using xggameplan.common.Caching;

namespace ImagineCommunications.BusClient.Implementation.Services
{
    public class MessageTypeService : IMessageTypeService
    {
        private const string CACHE_KEY = "xg-priorities";

        private readonly ICache _cache;
        private readonly IMessagePriorityRepository _priorityRepository;

        public MessageTypeService(IMessagePriorityRepository priorityRepository, ICache cache)
        {
            _cache = cache;
            _priorityRepository = priorityRepository;
        }

        public MessageType GetMessageType(string name)
        {
            var cache = _cache.Get(
                    CACHE_KEY,
                    () => _priorityRepository.GetAll().ToDictionary(c => c.Id)
                );

            MessageType type;

            return cache.TryGetValue(name, out type)
                ? type
                : null;
        }
    }
}
