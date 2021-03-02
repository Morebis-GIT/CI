using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.TotalRatings;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.TotalRatings;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class TotalRatingController : BaseController<IEvent, IEvent, IEvent, IBulkTotalRatingCreated, IBulkEvent<IEvent>, IBulkTotalRatingDeleted>
    {
        public TotalRatingController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        public async Task<IHttpActionResult> BulkCreate(BulkTotalRatingCreated bulkCreateModel, Guid groupTransactionId) => await base.BulkCreate(bulkCreateModel, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkTotalRatingDeleted deleteModel, Guid groupTransactionId) => await base.BulkDelete(deleteModel, groupTransactionId);
    }
}
