using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Breaks
{
    public class BulkBreakDeletedBatchingHandler : IBatchingHandler<IBulkBreaksDeleted>
    {
        private readonly IEventHandler<IBulkBreaksDeleted> _eventHandler;
        private readonly IMessageInfoRepository _messageInfoRepository;
        public BulkBreakDeletedBatchingHandler(IEventHandler<IBulkBreaksDeleted> eventHandler, IMessageInfoRepository messageInfoRepository)
        {
            _eventHandler = eventHandler;
            _messageInfoRepository = messageInfoRepository;
        }

        public void Handle(MessageInfo info, IBulkBreaksDeleted command)
        {
            if (info.MessageType.BatchSize > 0 &&
                _eventHandler is IBatchingEnumerateHandler<IBulkBreaksDeleted, IBreakDeleted> batchingEnumerateHandler)
            {
                int batchNo = 1;
                command.Data.OrderBy(x => x.DateRangeStart).EnumerateBatches(info.MessageType.BatchSize.Value, enumerator =>
                {
                    if (batchNo <= (info.ProcessedBatchCount ?? 0))
                    {
                        while (enumerator.MoveNext()) { }
                    }
                    else
                    {
                        batchingEnumerateHandler.Handle(enumerator);

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
        }
    }

}
