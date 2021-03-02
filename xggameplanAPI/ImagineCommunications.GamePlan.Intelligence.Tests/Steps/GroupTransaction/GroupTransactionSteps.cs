using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Domain.Abstractions.Repositories;
using ImagineCommunications.BusClient.Domain.Entities;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.GroupTransaction;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces;
using MessagePriorityEntity = ImagineCommunications.Gameplan.Integration.Data.Entities.MessageType;
using MassTransit.Testing;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Steps.GroupTransaction
{
    [Binding, Scope(Tag = "GroupTransaction")]
    public class GroupTransactionSteps
    {
        private readonly IServiceBus _serviceBus;
        private readonly IGroupTransactionExecutionService _groupTransactionExecutionService;
        private readonly IntelligenceDbContext _intelligenceDbContext;
        private readonly Dictionary<string, Guid> _groupTransactionIds = new Dictionary<string, Guid>();

        protected readonly InMemoryTestHarness InMemoryTestHarness;
        protected readonly IGroupTransactionInfoRepository GroupTransactionInfoRepository;
        protected readonly IMessageInfoRepository MessageInfoRepository;

        public GroupTransactionSteps(
            IntelligenceDbContext intelligenceDbContext,
            IServiceBus serviceBus,
            InMemoryTestHarness inMemoryTestHarness,
            IGroupTransactionInfoRepository groupTransactionInfoRepository,
            IMessageInfoRepository messageInfoRepository,
            IGroupTransactionExecutionService groupTransactionExecutionService)
        {
            _serviceBus = serviceBus;
            InMemoryTestHarness = inMemoryTestHarness;
            GroupTransactionInfoRepository = groupTransactionInfoRepository;
            MessageInfoRepository = messageInfoRepository;
            _groupTransactionExecutionService = groupTransactionExecutionService;
            _intelligenceDbContext = intelligenceDbContext;

            InsertPriorities();
        }

        [Given(@"I publish (.*) message with GroupTransaction (.*)")]
        public void GivenIPublishMessage(string eventName, string groupTransactionKey, Table table)
        {
            switch (eventName)
            {
                case nameof(IMockEventOne):
                    _ = _serviceBus.PublishAsync<IMockEventOne>(table.CreateInstance<MockEventOne>(), _groupTransactionIds[groupTransactionKey]).GetAwaiter().GetResult();
                    _ = InMemoryTestHarness.Consumed.Select<IMockEventOne>(t => t.Exception == null).Any();
                    break;
                case nameof(IMockEventTwo):
                    _ = _serviceBus.PublishAsync<IMockEventTwo>(table.CreateInstance<MockEventTwo>(), _groupTransactionIds[groupTransactionKey]).GetAwaiter().GetResult();
                    _ = InMemoryTestHarness.Consumed.Select<IMockEventTwo>(t => t.Exception == null).Any();
                    break;
                case nameof(IMockEventThree):
                    _ = _serviceBus.PublishAsync<IMockEventThree>(table.CreateInstance<MockEventThree>(), _groupTransactionIds[groupTransactionKey]).GetAwaiter().GetResult();
                    _ = InMemoryTestHarness.Consumed.Select<IMockEventThree>(t => t.Exception == null).Any();
                    break;
                case nameof(IMockEventFour):
                    _ = _serviceBus.PublishAsync<IMockEventFour>(table.CreateInstance<MockEventFour>(), _groupTransactionIds[groupTransactionKey]).GetAwaiter().GetResult();
                    _ = InMemoryTestHarness.Consumed.Select<IMockEventFour>(t => t.Exception == null).Any();
                    break;
                default: throw new Exception("Invalid Type");
            }
        }

        [Then(@"messages are consumed and stored in local storage")]
        public void ThenMessagesAreConsumed()
        {
            var allconsumed = !InMemoryTestHarness.Consumed.Select(c => c.Exception != null).Any();
            Assert.True(allconsumed);
        }

        [Then(@"the table of GroupTransactionInfo will be updated as following")]
        public void ThenTheTableOfGroupTransactionInfoWillBeUpdatedAsFollowing(Table table)
        {
            var grpTransactions = table.CreateSet<GroupTransactionInfo>();
            var dbgrpTransactions = _intelligenceDbContext.GroupTransactionInfos.ToList();
            var grpTransactionsGrouped = grpTransactions.GroupBy(c => new
            {
                c.EventCount,
                c.State
            });
            var dbgrpTransactionsGrouped = dbgrpTransactions.GroupBy(c => new
            {
                c.EventCount,
                c.State
            });
            var jointTransactions = grpTransactionsGrouped.Join(dbgrpTransactionsGrouped,
                 local => new { local.Key.EventCount, local.Key.State, count = local.Count() },
                 db => new { db.Key.EventCount, db.Key.State, count = db.Count() },
                 (local, db) => local.Key);

            Assert.True(grpTransactions.Count() == dbgrpTransactions.Count() && grpTransactionsGrouped.Count() == dbgrpTransactionsGrouped.Count() && dbgrpTransactionsGrouped.Count() == jointTransactions.Count());
        }

        [Given(@"I publish GroupTransactionInfo and store returned Id as (.*)")]
        public void GivenIPublishGroupTransactionInfoAndStoreReturnedIdAs(string groupTransactionKey, Table table)
        {
            var groupTransactionObject = table.CreateInstance<GroupTransactionEvent>();
            var grpTransactionId = _serviceBus.PublishAsync<IGroupTransactionEvent>(groupTransactionObject).GetAwaiter().GetResult();
            _groupTransactionIds.Add(groupTransactionKey, grpTransactionId);
            _ = InMemoryTestHarness.Consumed.Select<IGroupTransactionEvent>(t => t.Exception == null).Any();
        }

        [Then(@"the GroupTransaction (.*) will be updated as following")]
        public void ThenTheGroupTransactionWillBeUpdatedAsFollowing(string groupTransactionKey, Table table)
        {
            var groupTransactionId = _groupTransactionIds[groupTransactionKey];
            var groupTransactionObject = table.CreateInstance<GroupTransactionInfo>();
            var groupTransactionFromDB = GroupTransactionInfoRepository.GetById(groupTransactionId);
            Assert.True(groupTransactionFromDB.State == groupTransactionObject.State && groupTransactionFromDB.EventCount == groupTransactionObject.EventCount);
        }

        [Then(@"the MessageInfo with GroupTransaction (.*) will be updated as following")]
        public void ThenTheMessageInfoWithGroupTransactionWillBeUpdatedAsFollowing(string groupTransactionKey, Table table)
        {
            var dbmessageInfos = MessageInfoRepository.GetByTransactionId(_groupTransactionIds[groupTransactionKey]);
            var messageInfos = table.CreateSet<MessageInfo>();
            var dbmessageInfosGrouped = dbmessageInfos.GroupBy(m => new { m.Name, m.State, m.Type });
            var messageInfosGrouped = messageInfos.GroupBy(m => new { m.Name, m.State, m.Type });
            var joinedMessages = dbmessageInfosGrouped.Join(messageInfosGrouped,
                db => db.Key,
                local => local.Key,
                (db, local) => db.Key);
            Assert.True(dbmessageInfos.Count() == messageInfos.Count() && dbmessageInfosGrouped.Count() == messageInfosGrouped.Count() && messageInfosGrouped.Count() == joinedMessages.Count());
        }

        [Then(@"start GroupTransactionExecutor service and wait for completion")]
        public void ThenStartProcessingTheMessages()
        {
            var handlerSuccessfullyExecuted = true;
            Exception exception = null;
            _groupTransactionExecutionService.OnError = null;
            _groupTransactionExecutionService.OnError += (object sender, Exception ex) =>
            {
                handlerSuccessfullyExecuted = false;
                exception = ex;
            };
            bool executed = false;
            while (!executed)
            {
                executed = _groupTransactionExecutionService.Execute();
            }
        }

        //Helper Methods
        private void InsertPriorities()
        {
            if (!_intelligenceDbContext.MessageTypes.Any())
            {
                var data = new Gameplan.Integration.Data.Entities.MessageEntityType() { Name = "Entity" };

                _ = _intelligenceDbContext.MessageEntityTypes.Add(data);

                _ = _intelligenceDbContext.MessageTypes.Add(new MessagePriorityEntity { Id = nameof(IMockEventOne), Priority = 1, MessageEntityType = data });
                _ = _intelligenceDbContext.MessageTypes.Add(new MessagePriorityEntity { Id = nameof(IMockEventTwo), Priority = 2, MessageEntityType = data });
                _ = _intelligenceDbContext.MessageTypes.Add(new MessagePriorityEntity { Id = nameof(IMockEventThree), Priority = 3, MessageEntityType = data });
                _ = _intelligenceDbContext.MessageTypes.Add(new MessagePriorityEntity { Id = nameof(IMockEventFour), Priority = 4, MessageEntityType = data });
                _ = _intelligenceDbContext.SaveChanges();
            }
        }
    }
}
