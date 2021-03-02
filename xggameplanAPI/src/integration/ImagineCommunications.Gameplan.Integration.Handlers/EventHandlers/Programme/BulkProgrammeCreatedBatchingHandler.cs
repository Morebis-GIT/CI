using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme
{
    internal class BulkProgrammeCreatedBatch : IBulkProgrammeCreated
    {
        public IEnumerable<IProgrammeCreated> Data { get; }

        public BulkProgrammeCreatedBatch(IEnumerable<IProgrammeCreated> data)
        {
            Data = data;
        }
    }

    public class BulkProgrammeCreatedBatchingHandler : IBatchingHandler<IBulkProgrammeCreated>
    {
        private readonly IEventHandler<IBulkProgrammeCreated> _eventHandler;
        private readonly IMessageInfoRepository _messageInfoRepository;

        public BulkProgrammeCreatedBatchingHandler(IEventHandler<IBulkProgrammeCreated> eventHandler, IMessageInfoRepository messageInfoRepository)
        {
            _eventHandler = eventHandler;
            _messageInfoRepository = messageInfoRepository;
        }

        public void Handle(MessageInfo info, IBulkProgrammeCreated command)
        {
            if (info.MessageType.BatchSize > 0 &&
                _eventHandler is IBatchingEnumerateHandler<IBulkProgrammeCreated, IProgrammeCreated>
                    batchingEnumerateHandler)
            {
                var batchNo = 1;
                command.Data.OrderBy(c => c.StartDateTime).EnumerateBatches(info.MessageType.BatchSize.Value,
                    enumerator =>
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
