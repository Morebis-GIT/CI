using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign
{
    internal class BulkCampaignCreatedOrUpdatedBatch : IBulkCampaignCreatedOrUpdated
    {
        public IEnumerable<ICampaignCreatedOrUpdated> Data { get; }

        public BulkCampaignCreatedOrUpdatedBatch(IEnumerable<ICampaignCreatedOrUpdated> data)
        {
            Data = data;
        }
    }

    public class CampaignCreatedOrUpdatedBatchingHandler : IBatchingHandler<IBulkCampaignCreatedOrUpdated>
    {
        private readonly IBulkCampaignCreatedOrUpdatedEventHandler _eventHandler;
        private readonly IMessageInfoRepository _messageInfoRepository;
        private readonly ILoggerService _logger;

        public CampaignCreatedOrUpdatedBatchingHandler(IBulkCampaignCreatedOrUpdatedEventHandler eventHandler, IMessageInfoRepository messageInfoRepository, ILoggerService logger)
        {
            _eventHandler = eventHandler;
            _messageInfoRepository = messageInfoRepository;
            _logger = logger;
        }

        public void Handle(MessageInfo info, IBulkCampaignCreatedOrUpdated command)
        {
            if (!info.MessageType.BatchSize.HasValue)
            {
                _eventHandler.Handle(command);
            }
            else
            {
                _logger.Info($"Event: CampaignCreatedOrUpdatedBatchingHandler create/update process is started; MessageId: {info.Id}");
                var totalBatchSize = (int)Math.Ceiling((decimal)command.Data.Count() / (decimal)info.MessageType.BatchSize);
                info.TotalBatchCount = totalBatchSize;
                while ((info.ProcessedBatchCount ?? 0) < (info.TotalBatchCount ?? 0))
                {
                    var batch = command.Data.GetBatches(info.MessageType.BatchSize ?? 0, info.ProcessedBatchCount ?? 0);
                    _eventHandler.HandleWithoutLibraryScenario(new BulkCampaignCreatedOrUpdatedBatch(batch));
                    info.ProcessedBatchCount = (info.ProcessedBatchCount ?? 0) + 1;
                    _messageInfoRepository.Update(info);
                }
                _logger.Info($"Event: CampaignCreatedOrUpdatedBatchingHandler create/update process is finished; MessageId: {info.Id}");

                _logger.Info($"Event: CampaignCreatedOrUpdatedBatchingHandler handle library scenario process is started; MessageId: {info.Id}");
                _eventHandler.HandleLibraryScenario();
                _logger.Info($"Event: CampaignCreatedOrUpdatedBatchingHandler handle library scenario process is finished; MessageId: {info.Id}");
            }
        }
    }
}
