using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.InventoryStatus.LockType;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.InventoryStatus.LockType;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class LockTypeController : BaseController<IEvent, IEvent, IBulkLockTypeDeleted, IBulkLockTypeCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public LockTypeController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkLockTypeCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkLockTypeDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
