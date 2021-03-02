using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class ProgrammeController : BaseController<IEvent, IEvent, IEvent, IBulkProgrammeCreated, IBulkProgrammeUpdated, IBulkProgrammeDeleted>
    {
        public ProgrammeController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkProgrammeCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkUpdate(BulkProgrammeUpdated model, Guid groupTransactionId) => await base.BulkUpdate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkProgrammeDeleted model, Guid groupTransactionId) => await base.BulkDelete(model, groupTransactionId);
    }
}
