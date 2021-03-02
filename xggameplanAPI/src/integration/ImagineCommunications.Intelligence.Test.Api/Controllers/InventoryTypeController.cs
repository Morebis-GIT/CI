using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryType;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryType;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class InventoryTypeController : BaseController<IEvent, IEvent, IBulkInventoryTypeDeleted, IBulkInventoryTypeCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public InventoryTypeController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkInventoryTypeCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkInventoryTypeDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
