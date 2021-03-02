using System.Threading.Tasks;
using System.Web.Http;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.GroupTransaction;

namespace ImagineCommunications.Intelligence.Test.Api.Controllers
{
    public class GroupTransactionController : BaseController<IGroupTransactionEvent, IEvent, IEvent, IBulkEvent<IEvent>, IBulkEvent<IEvent>, IEvent>
    {
        public GroupTransactionController(IServiceBus serviceBus) : base(serviceBus) { }

        public async Task<IHttpActionResult> Create(int eventCount)
        {
            return await base.Create(new GroupTransactionEvent(eventCount));
        }
    }
}
