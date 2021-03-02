using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor;
using ImagineCommunications.GamePlan.Domain.LengthFactors;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.LengthFactors
{
    public class BulkLengthFactorDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkLengthFactorDeleted>
    {
        private readonly ILengthFactorRepository _repository;

        public BulkLengthFactorDeletedEventHandler(ILengthFactorRepository repository)
        {
            _repository = repository;
        }

        public override void Handle(IBulkLengthFactorDeleted command)
        {
            _repository.Truncate();
            _repository.SaveChanges();
        }
    }
}
