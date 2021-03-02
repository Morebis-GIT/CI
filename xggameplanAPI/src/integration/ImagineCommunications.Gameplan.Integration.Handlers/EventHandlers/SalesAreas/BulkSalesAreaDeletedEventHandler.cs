using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreaDemographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.SalesAreas
{
    public class BulkSalesAreaDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkSalesAreaDeleted>
    {
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly ISalesAreaDemographicRepository _salesAreaDemographicRepository;

        public BulkSalesAreaDeletedEventHandler(
            ISalesAreaRepository salesAreaRepository,
            ISalesAreaDemographicRepository salesAreaDemographicRepository)
        {
            _salesAreaRepository = salesAreaRepository;
            _salesAreaDemographicRepository = salesAreaDemographicRepository;
        }

        public override void Handle(IBulkSalesAreaDeleted command)
        {
            var salesAreas = _salesAreaRepository.FindByShortNames(command.Data.Select(c => c.ShortName));
            if (salesAreas.Any())
            {
                _salesAreaRepository.DeleteRange(salesAreas.Select(c=>c.Id));
                _salesAreaDemographicRepository.DeleteBySalesAreaNames(salesAreas.Select(c => c.Name));

                _salesAreaRepository.SaveChanges();
            }
        }
    }
}
