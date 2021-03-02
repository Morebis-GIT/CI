using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SpotBookingRules;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SpotBookingRules;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class SpotBookingRuleController : BaseController<IEvent, IEvent, IBulkSpotBookingRuleDeleted, IBulkSpotBookingRuleCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public SpotBookingRuleController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkSpotBookingRuleCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkSpotBookingRuleDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}