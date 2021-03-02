using System;
using System.Collections.Generic;
using ImagineCommunications.BusClient.Domain.Entities;

namespace ImagineCommunications.BusClient.Domain.Abstractions.Repositories
{
    public interface IGroupTransactionInfoRepository
    {
        GroupTransactionInfo GetById(Guid id);
        GroupTransactionInfo GetLatestExecutedGroupTransaction();
        List<GroupTransactionInfo> GetTransactionsToExecute(int transactionsLimit = 10);

        void Add(GroupTransactionInfo entity);
        void Update(GroupTransactionInfo entity);
    }
}
