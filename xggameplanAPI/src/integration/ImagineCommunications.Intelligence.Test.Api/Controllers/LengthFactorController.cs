using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.LengthFactor;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class LengthFactorController : BaseController<IEvent, IEvent, IBulkLengthFactorDeleted, IBulkLengthFactorCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public LengthFactorController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkLengthFactorCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkLengthFactorDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
