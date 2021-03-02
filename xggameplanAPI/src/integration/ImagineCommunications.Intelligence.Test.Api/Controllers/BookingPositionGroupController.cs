using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class BookingPositionGroupController : BaseController<IEvent, IEvent, IBookingPositionGroupTruncated, IBulkBookingPositionGroupCreated, IBulkEvent<IEvent>, IBulkBookingPositionGroupDeleted>
    {
        public BookingPositionGroupController(IServiceBus serviceBus) : base(serviceBus) { }

        public async Task<IHttpActionResult> BulkCreate(BulkBookingPositionGroupCreated model, Guid groupTransactionId) => await base.BulkCreate(model, groupTransactionId);

        public async Task<IHttpActionResult> BulkDelete(BulkBookingPositionGroupDeleted model, Guid groupTransactionId) => await base.BulkDelete(model, groupTransactionId);

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAll(Guid groupTransactionId) => await base.Delete(new BookingPositionGroupTruncated(), groupTransactionId);
    }
}
