using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme
{
    public class BulkProgrammeDeletedBatchingHandler : IBatchingHandler<IBulkProgrammeDeleted>
    {
        private readonly IEventHandler<IBulkProgrammeDeleted> _eventHandler;
        private readonly IMessageInfoRepository _messageInfoRepository;

        public BulkProgrammeDeletedBatchingHandler(IEventHandler<IBulkProgrammeDeleted> eventHandler, IMessageInfoRepository messageInfoRepository)
        {
            _eventHandler = eventHandler;
            _messageInfoRepository = messageInfoRepository;
        }

        public void Handle(MessageInfo info, IBulkProgrammeDeleted command)
        {
            if (info.MessageType.BatchSize > 0 &&
                _eventHandler is IBatchingEnumerateHandler<IBulkProgrammeDeleted, IProgrammesDeleted>
                    batchingEnumerateHandler)
            {
                var batchNo = 1;
                command.Data.GroupBy(p => p.SalesArea).EnumerateBatches(info.MessageType.BatchSize.Value,
                    enumerator =>
                    {
                        if (batchNo <= (info.ProcessedBatchCount ?? 0))
                        {
                            while (enumerator.MoveNext()) { }
                        }
                        else
                        {
                            using (var batchEnumerator = GetBatchEnumerable(enumerator).GetEnumerator())
                            {
                                batchingEnumerateHandler.Handle(batchEnumerator);
                            }

                            info.ProcessedBatchCount = batchNo;
                            _messageInfoRepository.Update(info);
                        }

                        batchNo++;
                    });
            }
            else
            {
                _eventHandler.Handle(command);
            }

            IEnumerable<IProgrammesDeleted> GetBatchEnumerable(
                IEnumerator<IGrouping<string, IProgrammesDeleted>> enumerator)
            {
                while (enumerator.MoveNext())
                {
                    foreach (var item in enumerator.Current.OrderBy(x => x.FromDate))
                    {
                        yield return item;
                    }
                }
            }
        }
    }
}
