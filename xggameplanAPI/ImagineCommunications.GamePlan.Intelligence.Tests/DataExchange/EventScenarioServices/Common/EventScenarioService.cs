using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using BoDi;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.GroupTransaction;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using MassTransit.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.EventScenarioServices.Common
{
    public class EventScenarioService<TEvent, TEventModel> : IEventScenarioService
        where TEvent : class, IEvent where TEventModel : class, IEvent, TEvent
    {
        private readonly ITestOutputHelper _output;
        protected readonly IGroupTransactionExecutionService _groupTransactionExecutionService;
        protected readonly IObjectContainer _objectContainer;
        protected readonly InMemoryTestHarness _inMemoryTestHarness;
        protected TEventModel _message { get; set; }
        private readonly IScenarioDbContext _scenarioDbContext;
        private readonly IServiceBus _serviceBus;
        public Guid? GroupTransactionId { get; set; }

        public EventScenarioService(InMemoryTestHarness inMemoryTestHarness,
            IObjectContainer objectContainer,
            IScenarioDbContext scenarioDbContext,
            IServiceBus serviceBus,
            IGroupTransactionExecutionService groupTransactionExecutionService,
            ITestOutputHelper output)
        {
            _inMemoryTestHarness = inMemoryTestHarness;
            _objectContainer = objectContainer;
            _scenarioDbContext = scenarioDbContext;
            _serviceBus = serviceBus;
            _groupTransactionExecutionService = groupTransactionExecutionService;
            _output = output;
        }

        public virtual void CreateEventModel(Table table)
        {
            _message = table.CreateInstance<TEventModel>();
        }

        public virtual void CreateEventModelFromFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(
                assembly.GetFullManifestResourceName(fileName.EndsWith(".json") ? fileName : fileName + ".json")))
            {
                using (var jsonReader = new JsonTextReader(new StreamReader(stream, Encoding.UTF8)))
                {
                    var serializer = JsonSerializer.CreateDefault(new JsonSerializerSettings());
                    _message = serializer.Deserialize<TEventModel>(jsonReader);
                }
            }
        }

        public virtual void PublishGroupTransaction()
        {
            GroupTransactionId = _serviceBus.PublishAsync<IGroupTransactionEvent>(new GroupTransactionEvent(1)).GetAwaiter().GetResult();
        }

        public virtual void PublishMessage()
        {
            var grpTransactionResult = _inMemoryTestHarness.Consumed.Select<IGroupTransactionEvent>(s => s.Exception == null).Any();
            _ = _serviceBus.PublishAsync<TEvent>(_message, GroupTransactionId).GetAwaiter().GetResult();
        }

        public virtual bool CheckConsumerSuccessfullyConsumed()
        {
            _scenarioDbContext.WaitForIndexesAfterSaveChanges();
            var result = _inMemoryTestHarness.Consumed.Select<TEvent>(s => s.Exception == null).Any();
            _groupTransactionExecutionService.OnError = null;
            _groupTransactionExecutionService.OnError += (object sender, Exception exception) =>
              {
                  result = false;
              };

            bool executed = false;
            while (!executed)
            {
                executed = _groupTransactionExecutionService.Execute();
            }

            _scenarioDbContext.SaveChanges();
            _scenarioDbContext.WaitForIndexesToBeFresh();
            return result;
        }

        public virtual bool CheckDataSyncErrorCode(string errorCode)
        {
            _scenarioDbContext.WaitForIndexesAfterSaveChanges();
            var successfullyConsumed = _inMemoryTestHarness.Consumed.Select<TEvent>(s => s.Exception == null).Any();
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

            _scenarioDbContext.WaitForIndexesToBeFresh();
            if (!successfullyConsumed && handlerSuccessfullyExecuted)
            {
                return false;
            }
            else
            {
                var dataSyncException = exception as DataSyncException;
                if (dataSyncException.ErrorCode.ToString() == errorCode)
                {
                    return true;
                }

                return false;
            }
        }

        public virtual bool CheckContractValidationFields(Table fields)
        {
            _scenarioDbContext.WaitForIndexesAfterSaveChanges();
            var successfullyConsumed = _inMemoryTestHarness.Consumed.Select<TEvent>(s => s.Exception == null).Any();
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

            _scenarioDbContext.WaitForIndexesToBeFresh();
            _output.WriteLine("successfullyConsumed: " + successfullyConsumed.ToString() + " handlerSuccessfullyExecuted: " + handlerSuccessfullyExecuted.ToString());
            if (!successfullyConsumed && handlerSuccessfullyExecuted)
            {
                return false;
            }
            else
            {
                var validationException = (exception as ContractValidationException);
                if (validationException == null)
                {
                    _output.WriteLine(exception.GetType().FullName);
                }

                var errorFields = validationException.Errors.Select(s => s.PropertyName).OrderBy(s => s);
                var potentialFields = fields.Rows.Select(r => r["PropertyName"]).ToArray().OrderBy(s => s);
                return errorFields.SequenceEqual(potentialFields);
            }
        }
    }

    public class EventScenarioService<TEvent, TEventModel, TBulkEventListItem, TBulkEventListItemModel> : EventScenarioService<TEvent, TEventModel>
        where TEvent : class, IEvent
        where TEventModel : class, IEvent, TEvent
        where TBulkEventListItem : IEvent
        where TBulkEventListItemModel : IEvent
    {
        public EventScenarioService(InMemoryTestHarness inMemoryTestHarness,
            IObjectContainer objectContainer,
            IScenarioDbContext scenarioDbContext,
            IServiceBus serviceBus,
            IGroupTransactionExecutionService groupTransactionExecutionService,
            ITestOutputHelper output) :
            base(inMemoryTestHarness, objectContainer, scenarioDbContext, serviceBus, groupTransactionExecutionService, output)
        {
        }

        public override void CreateEventModel(Table table)
        {
            if (typeof(TEvent).GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IBulkEvent<>)))
            {
                var containerTable = new Table("Data");
                var container = containerTable.CreateInstance<TEventModel>();
                var jObject = JObject.FromObject(container);
                jObject.Add("data", JToken.FromObject(table.CreateSet<TBulkEventListItemModel>()));
                _message = jObject.ToObject<TEventModel>();
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}
