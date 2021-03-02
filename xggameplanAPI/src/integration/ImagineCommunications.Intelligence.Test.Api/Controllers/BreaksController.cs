using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Breaks;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class BreaksController : BaseController<IEvent, IEvent, IEvent, IBulkBreakCreated, IBulkEvent<IEvent>, IBulkBreaksDeleted>
    {
        public BreaksController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkBreaksDeleted deleteModel, Guid groupTransactionId)
        {
            return await base.BulkDelete(deleteModel, groupTransactionId);
        }

        public async Task<IHttpActionResult> BulkCreate(BulkBreakCreated bulkCreateModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(bulkCreateModel, groupTransactionId);
        }
    }
}
