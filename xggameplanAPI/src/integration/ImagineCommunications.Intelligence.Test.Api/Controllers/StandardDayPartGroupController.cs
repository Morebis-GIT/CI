using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.DayPartGroups;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class StandardDayPartGroupController : BaseController<IEvent, IEvent, IBulkStandardDayPartGroupDeleted, IBulkStandardDayPartGroupCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public StandardDayPartGroupController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkStandardDayPartGroupCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkStandardDayPartGroupDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
