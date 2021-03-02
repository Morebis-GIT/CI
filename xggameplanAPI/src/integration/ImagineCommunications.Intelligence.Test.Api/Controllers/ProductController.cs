using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Product;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class ProductController : BaseController<IEvent, IEvent, IEvent,
        IBulkProductCreatedOrUpdated, IBulkEvent<IEvent>, IBulkProductDeleted>
    {
        public ProductController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        public async Task<IHttpActionResult> BulkCreateOrUpdate(BulkProductCreatedOrUpdated createModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(createModel, groupTransactionId);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkProductDeleted createModel, Guid groupTransactionId)
        {
            return await base.BulkDelete(createModel, groupTransactionId);
        }
    }
}
