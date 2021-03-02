using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class SalesAreaController : BaseController<IEvent, ISalesAreaUpdated, IBulkSalesAreaDeleted, IBulkSalesAreaCreatedOrUpdated, IBulkEvent<IEvent>, IBulkSalesAreaDeleted>
    {
        public SalesAreaController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        public async Task<IHttpActionResult> BulkCreateOrUpdate(BulkSalesAreaCreatedOrUpdated createModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(createModel, groupTransactionId);
        }

        public async Task<IHttpActionResult> Update(SalesAreaUpdated updateModel, Guid groupTransactionId)
        {
            return await base.Update(updateModel, groupTransactionId);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkSalesAreaDeleted updateModel, Guid groupTransactionId)
        {
            return await base.Delete(updateModel, groupTransactionId);
        }
    }
}
