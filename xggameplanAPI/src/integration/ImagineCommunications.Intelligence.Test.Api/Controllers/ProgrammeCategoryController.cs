using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeCategory;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeCategory;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class ProgrammeCategoryController : BaseController<IEvent, IEvent,  IBulkProgrammeCategoryDeleted, IBulkProgrammeCategoryCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {
        public ProgrammeCategoryController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkProgrammeCategoryCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkProgrammeCategoryDeleted model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
