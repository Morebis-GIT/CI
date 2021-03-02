using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Domain.Abstractions.Repositories
{
    public interface IMessageInfoRepository 
    {
        IEnumerable<MessageInfo> GetByTransactionId(Guid transactionId);
        IEnumerable<MessageInfo> GetByTransactionIds(IEnumerable<Guid> transactionIds);
        void Add(MessageInfo entity);
        void Update(MessageInfo entity);
        void Delete(Guid id);
    }
}
