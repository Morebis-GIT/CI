using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayParts;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.DayParts;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class StandardDayPartController : BaseController<IEvent, IEvent, IBulkStandardDayPartDeleted, IBulkStandardDayPartCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public StandardDayPartController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkStandardDayPartCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkStandardDayPartDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
