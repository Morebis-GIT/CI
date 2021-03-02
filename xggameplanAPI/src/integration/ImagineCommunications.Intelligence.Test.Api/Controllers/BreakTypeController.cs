using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.BreakTypes;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class BreakTypeController : BaseController<IEvent, IEvent, IBulkBreakTypeDeleted, IBulkBreakTypeCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public BreakTypeController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkBreakTypeCreated bulkCreateModel,
            Guid groupTransactionId)
            => await base.BulkCreate(bulkCreateModel, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkBreakTypeDeleted deleteModel,
            Guid groupTransactionId) => await base.Delete(deleteModel, groupTransactionId);
    }
}
