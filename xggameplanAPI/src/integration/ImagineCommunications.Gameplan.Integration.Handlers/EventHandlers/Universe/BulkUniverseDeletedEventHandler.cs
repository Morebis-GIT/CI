using ImagineCommunications.BusClient.Abstraction.Classes;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Universe
{
    public class BulkUniverseDeletedEventHandler : EventHandler<IBulkUniverseDeleted>
    {
        private readonly IUniverseRepository _universeRepository;

        public BulkUniverseDeletedEventHandler(IUniverseRepository universeRepository) => _universeRepository = universeRepository;

        public override void Handle(IBulkUniverseDeleted command)
        {
            foreach (var item in command.Data)
            {
                _universeRepository.DeleteByCombination(item.SalesArea, item.Demographic, item.StartDate, item.EndDate);
            }

            _universeRepository.SaveChanges();
        }
    }
}
