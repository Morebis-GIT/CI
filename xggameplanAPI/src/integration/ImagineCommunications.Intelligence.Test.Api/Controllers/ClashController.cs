using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class ClashController : BaseController<IClashTruncated, IClashUpdated, IBulkClashDeleted, IBulkClashCreatedOrUpdated, IBulkEvent<IEvent>, IBulkClashDeleted>
    {
        public ClashController(IServiceBus serviceBus) : base(serviceBus) { }

        public async Task<IHttpActionResult> BulkCreate(BulkClashCreatedOrUpdated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        public async Task<IHttpActionResult> Update(ClashUpdated model, Guid groupTransactionId) => await base.Update(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkClashDeleted model, Guid groupTransactionId) => await base.BulkDelete(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkTruncate(ClashTruncated model, Guid groupTransactionId) => await base.Create(model, groupTransactionId);
    }
}
