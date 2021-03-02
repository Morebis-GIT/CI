using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ClashExceptions;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.ClashExceptions;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class ClashExceptionController : BaseController<IEvent, IEvent, IBulkClashExceptionDeleted, IBulkClashExceptionCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public ClashExceptionController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkClashExceptionDeleted deleteModel, Guid groupTransactionId) => await base.BulkDelete(deleteModel, groupTransactionId);

        public async Task<IHttpActionResult> BulkCreate(BulkClashExceptionCreated bulkCreateModel, Guid groupTransactionId) => await base.BulkCreate(bulkCreateModel, groupTransactionId);
    }
}
