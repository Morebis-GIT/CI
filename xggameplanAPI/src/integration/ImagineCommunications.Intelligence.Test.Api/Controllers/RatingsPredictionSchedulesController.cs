using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.RatingsPredictionSchedules;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class RatingsPredictionSchedulesController : BaseController<IEvent, IEvent, IEvent, IBulkRatingsPredictionSchedulesCreated, IBulkEvent<IEvent>, IBulkRatingsPredictionSchedulesDeleted>
    {
        public RatingsPredictionSchedulesController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        public async Task<IHttpActionResult> BulkCreate(BulkRatingsPredictionScheduleCreated bulkCreateModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(bulkCreateModel, groupTransactionId);
        }

        [HttpPost]
        public async Task<IHttpActionResult> Delete(BulkRatingsPredictionSchedulesDeleted deleteModel, Guid groupTransactionId)
        {
            return await base.BulkDelete(deleteModel, groupTransactionId);
        }
    }
}
