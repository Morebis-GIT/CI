using System.Collections.Generic;

namespace ImagineCommunications.BusClient.Domain.Abstractions.Repositories
{
    public interface IMessagePriorityRepository
    {
        Entities.MessageType GetById(string id);

        List<Entities.MessageType> GetAll();
    }
}
