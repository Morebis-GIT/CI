using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.InventoryLock;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.InventoryLock;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class InventoryLockController : BaseController<IEvent, IEvent, IBulkInventoryLockDeleted, IBulkInventoryLockCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public InventoryLockController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkInventoryLockCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkInventoryLockDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
