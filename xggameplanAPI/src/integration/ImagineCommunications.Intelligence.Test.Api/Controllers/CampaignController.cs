using System;
using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class CampaignController : BaseController<IEvent, IEvent, IEvent, IBulkCampaignCreatedOrUpdated, IBulkEvent<IEvent>, IBulkCampaignDeleted>
    {
        public CampaignController(IServiceBus serviceBus) : base(serviceBus)
        {
        }

        public async Task<IHttpActionResult> BulkCreate(BulkCampaignCreatedOrUpdated createModel, Guid groupTransactionId)
        {
            return await base.BulkCreate(createModel, groupTransactionId);
        }

        [HttpPost]
        public async Task<IHttpActionResult> BulkDelete(BulkCampaignDeleted deleteModel, Guid groupTransactionId)
        {
            return await base.BulkDelete(deleteModel, groupTransactionId);
        }
    }
}
