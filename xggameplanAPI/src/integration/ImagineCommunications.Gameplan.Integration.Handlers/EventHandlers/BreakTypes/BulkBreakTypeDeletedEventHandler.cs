using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BreakTypes;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.BreakTypes
{
    public class BulkBreakTypeDeletedEventHandler : IEventHandler<IBulkBreakTypeDeleted>
    {
        private readonly IMetadataRepository _metadataRepository; 

        public BulkBreakTypeDeletedEventHandler(IMetadataRepository metadataRepository)
        {
            _metadataRepository = metadataRepository;
        }

        public void Handle(IBulkBreakTypeDeleted command)
        {
            _metadataRepository.DeleteByKey(MetaDataKeys.BreakTypes);
            _metadataRepository.SaveChanges();
        }
    }
}
