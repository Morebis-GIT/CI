using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Spot;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Spot;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class SpotController : BaseController<IEvent, IEvent, IEvent, IBulkSpotCreatedOrUpdated, IBulkEvent<IEvent>, IBulkSpotDeleted>
    {
        public SpotController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreateOrUpdate(BulkSpotCreatedOrUpdated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkSpotDeleted model, Guid groupTransactionId) => await base.BulkDelete(model, groupTransactionId);
    }
}
