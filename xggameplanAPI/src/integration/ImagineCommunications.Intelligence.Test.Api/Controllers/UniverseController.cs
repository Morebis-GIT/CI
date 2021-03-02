using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Universe;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class UniverseController : BaseController<IEvent, IEvent, IEvent, IBulkUniverseCreated, IBulkEvent<IEvent>, IBulkUniverseDeleted>
    {
        public UniverseController(IServiceBus serviceBus) : base(serviceBus) { }

        public async Task<IHttpActionResult> BulkCreate(BulkUniverseCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkUniverseDeleted model, Guid groupTransactionId) => await base.BulkDelete(model, groupTransactionId);
    }
}
