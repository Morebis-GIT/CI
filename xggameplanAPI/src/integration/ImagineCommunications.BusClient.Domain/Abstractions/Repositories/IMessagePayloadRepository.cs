using System;
using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Domain.Abstractions.Repositories
{
    public interface IMessagePayloadRepository
    {
        MessagePayload GetById(Guid id);

        void Add(MessagePayload entity);
        void DeleteById(Guid messageId);
    }
}
