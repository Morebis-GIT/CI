using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.ProgrammeClassification;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class ProgrammeClassificationController : BaseController<IEvent, IEvent, IDeleteAllProgrammeClassification, IBulkProgrammeClassificationCreated, IBulkEvent<IEvent>, IBulkEvent<IEvent>>
    {

        public ProgrammeClassificationController(IServiceBus serviceBus) : base(serviceBus) { }

        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(BulkProgrammeClassificationCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAll(DeleteAllProgrammeClassification model, Guid groupTransactionId) => await base.Delete(model, groupTransactionId);
    }
}
