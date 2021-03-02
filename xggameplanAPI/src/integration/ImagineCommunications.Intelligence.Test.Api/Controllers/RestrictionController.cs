using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Restriction;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Restriction;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class RestrictionController : BaseController<IEvent, IEvent, IEvent, IBulkRestrictionCreatedOrUpdated, IBulkEvent<IEvent>, IBulkRestrictionDeleted>
    {
        public RestrictionController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreateOrUpdate(BulkRestrictionCreatedOrUpdated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkRestrictionDeleted model, Guid groupTransactionId) => await base.BulkDelete(model, groupTransactionId);
    }
}
