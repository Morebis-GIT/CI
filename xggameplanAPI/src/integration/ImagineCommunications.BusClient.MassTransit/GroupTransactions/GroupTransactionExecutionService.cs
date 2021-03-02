using System;
using System.Linq;
using System.Threading;
using ImagineCommunications.BusClient.Abstraction.Helpers;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;
using ImagineCommunications.BusClient.Domain;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Extensions.DependencyInjection;
using ImagineCommunications.Gameplan.Synchronization;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;

namespace ImagineCommunications.BusClient.Implementation.GroupTransactions
{
    public class GroupTransactionExecutionService : IGroupTransactionExecutionService
    {
        public EventHandler<Exception> OnError { get; set; }
        private static readonly int LocalEventRetryCount = 2;
        private static readonly int GlobalEventRetryCount = 3;
        private readonly IMessageInfoRepository _eventInfoRepository;
        private readonly IGroupTransactionInfoRepository _groupTransactionRepository;

        private readonly IIndex<IHandlerDispatcher> _handlerDispatcherResolver;
        private readonly IIndex<IPayloadStorageProviderService> _payloadStorageProviderResolver;

        private readonly IBigMessageService _bigMessageService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IMessageInfoRepository _messageInfoRepository;
        private readonly ILoggerService _logger;
        private static object _lock = new object();

        public GroupTransactionExecutionService(
            IMessageInfoRepository eventInfoRepository,
            IGroupTransactionInfoRepository groupTransactionRepository,
            IIndex<IPayloadStorageProviderService> payloadStorageProviderResolver,
            IIndex<IHandlerDispatcher> handlerDispatcherResolver,
            IBigMessageService bigMessageService,
            IMessageInfoRepository messageInfoRepository,
            ISynchronizationService synchronizationService,
            ILoggerService logger)
        {
            _payloadStorageProviderResolver = payloadStorageProviderResolver;
            _eventInfoRepository = eventInfoRepository;
            _groupTransactionRepository = groupTransactionRepository;
            _handlerDispatcherResolver = handlerDispatcherResolver;
            _bigMessageService = bigMessageService;
            _synchronizationService = synchronizationService;
            _messageInfoRepository = messageInfoRepository;
            _logger = logger;
        }

        public bool Execute()
        {
            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    var transactionToExecute = _groupTransactionRepository.GetTransactionsToExecute();

                    if (transactionToExecute.Count == 0)
                    {
                        return true;
                    }

                    if (_synchronizationService.TryCapture(SynchronizedServiceType.DataSynchronization,
                        Guid.NewGuid(), out var token))
                    {
                        try
                        {
                            foreach (var transaction in transactionToExecute)
                            {
                                if (!ExecuteTransaction(transaction))
                                {
                                    break;
                                }
                            }

                            return true;
                        }
                        finally
                        {
                            _synchronizationService.Release(token);
                        }
                    }
                }
                catch (Exception e)
                {
                    OnError?.Invoke(this, e);

                    _logger?.Error(
                        $"Exception was thrown during execution of the '{nameof(IGroupTransactionExecutionService)}'",
                        e);
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }

            return false;
        }

        private bool ExecuteTransaction(GroupTransactionInfo transaction)
        {
            _logger?.Info($"Transaction execution is started TransactionId: {transaction.Id} ");

            transaction.State = MessageState.InProgress;
            transaction.ExecutedDate = DateTime.UtcNow;

            _groupTransactionRepository.Update(transaction);

            _logger?.Info($"Transaction State is updated to {transaction.State.ToString()} TransactionId: {transaction.Id} ");

            transaction.State = MessageState.Completed;

            var messages = _messageInfoRepository.GetByTransactionId(transaction.Id);

            foreach (var eventMessage in messages.OrderByDescending(x => x.Priority))
            {
                if (eventMessage.State == MessageState.Completed)
                {
                    continue;
                }

                if (eventMessage.RetryCount == GlobalEventRetryCount)
                {
                    transaction.State = MessageState.Failed;

                    break;
                }
                _logger?.Info($"Event: {eventMessage.Name} processing started; MessageId: {eventMessage.Id} ");
                eventMessage.State = MessageState.InProgress;
                eventMessage.ExecutedDate = DateTime.UtcNow;

                _messageInfoRepository.Update(eventMessage);

                ProcessMessageWithRetry(eventMessage);

                _eventInfoRepository.Update(eventMessage);

                if (eventMessage.State == MessageState.Failed)
                {
                    transaction.State = eventMessage.RetryCount >= GlobalEventRetryCount
                        ? MessageState.Failed
                        : MessageState.ReadyForRetry;

                    break;
                }

                _logger?.Info($"Event: {eventMessage.Name} processed successfully; MessageId: {eventMessage.Id} ");
            }

            if (transaction.State == MessageState.Completed)
            {
                transaction.CompletedDate = DateTime.UtcNow;
            }

            _groupTransactionRepository.Update(transaction);

            _logger?.Info($"Transaction execution is finished TransactionId: {transaction.Id}; Transaction State: {transaction.State.ToString()}");

            //We can't allow executing of the next transaction if the current is not completed successfuly
            return transaction.State == MessageState.Completed;
        }

        private void ProcessMessageWithRetry(MessageInfo eventMessage)
        {
            var attemptCount = 0;

            while (attemptCount < LocalEventRetryCount)
            {
                try
                {
                    RestoreAndExecuteEvent(eventMessage);

                    eventMessage.State = MessageState.Completed;
                    eventMessage.CompletedDate = DateTime.UtcNow;
                    //Todo:uncomment it after testing landmark integration
                    //DeletaMessagePayload(eventMessage);

                    break;
                }
                catch (Exception e)
                {
                    attemptCount = e.GetType().IsSubclassOfRawGeneric(typeof(DataSyncException<>)) || e.GetType().IsSubclassOfRawGeneric(typeof(ContractValidationException<>))
                        ? LocalEventRetryCount
                        : attemptCount + 1;

                    if (attemptCount == LocalEventRetryCount)
                    {
                        eventMessage.State = MessageState.Failed;
                        eventMessage.RetryCount++;

                        try
                        {
                            OnError?.Invoke(eventMessage, e);
                        }
                        catch (Exception ex)
                        {
                            _logger?.Error($"Exception was thrown during triggering of OnError Handler of Event: {eventMessage.Name}; MessageId: {eventMessage.Id} ", ex);
                        }

                        _logger?.Error($"Exception was thrown during processing Event: {eventMessage.Name}; MessageId: {eventMessage.Id} ", e);
                    }
                }
            }
        }

        private void DeletaMessagePayload(MessageInfo messageInfo)
        {
            var messageTypeName = messageInfo.Type == MessageBodyType.BigMessage
                ? typeof(IBigMessage).Name
                : messageInfo.Name;

            _payloadStorageProviderResolver.Resolve(messageTypeName)?.DeleteMessage(messageInfo.Id);
        }

        private void RestoreAndExecuteEvent(MessageInfo messageInfo)
        {
            var messageTypeName = messageInfo.Type == MessageBodyType.BigMessage
                ? typeof(IBigMessage).Name
                : messageInfo.Name;

            _logger?.Info($"Event: RestoreAndExecuteEvent load message payload is started; MessageId:{messageInfo.Id}");
            var messageDataProvider = _payloadStorageProviderResolver.Resolve(messageTypeName);
            var restoredMessage = messageDataProvider.GetMessageData(messageInfo.Id);

            object messagePayload = restoredMessage;

            if (messageInfo.Type == MessageBodyType.BigMessage)
            {
                messagePayload = _bigMessageService.GetMessageAsync(restoredMessage as IBigMessage).Result;
            }
            _logger?.Info($"Event: RestoreAndExecuteEvent load message payload is finished; MessageId:{messageInfo.Id}");

            var handlerDispatcher = _handlerDispatcherResolver.Resolve(messageInfo.Name);

            handlerDispatcher.Dispatch(messageInfo, messagePayload);
        }
    }
}
