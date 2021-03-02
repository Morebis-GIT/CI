using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Holidays;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class HolidaysController : BaseController<IEvent, IEvent, IEvent, IBulkHolidayCreated, IBulkEvent<IEvent>, IBulkHolidayDeleted>
    {
        public HolidaysController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        public async Task<IHttpActionResult> BulkCreate(BulkHolidayCreated bulkCreateModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(bulkCreateModel, groupTransactionId);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkHolidayDeleted bulkDeleteModel, Guid groupTransactionId)
        {
            return await base.BulkDelete(bulkDeleteModel, groupTransactionId);
        }
    }
}
