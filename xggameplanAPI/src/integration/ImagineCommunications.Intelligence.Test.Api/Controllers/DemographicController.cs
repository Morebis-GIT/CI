using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Demographics;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Demographics;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class DemographicController : BaseController<IEvent, IDemographicUpdated, IEvent, IBulkDemographicCreatedOrUpdated, IBulkEvent<IEvent>, IBulkDemographicDeleted>
    {
        public DemographicController(IServiceBus serviceBus) : base(serviceBus)
        {
        }
        public async Task<IHttpActionResult> BulkCreate(BulkDemographicCreatedOrUpdated bulkCreateModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(bulkCreateModel, groupTransactionId);
        }

        public async Task<IHttpActionResult> Update(DemographicUpdated updateModel, Guid groupTransactionId)
        {
            return await base.Update(updateModel, groupTransactionId);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkDemographicDeleted updateModel, Guid groupTransactionId)
        {
            return await base.BulkDelete(updateModel, groupTransactionId);
        }
    }
}
