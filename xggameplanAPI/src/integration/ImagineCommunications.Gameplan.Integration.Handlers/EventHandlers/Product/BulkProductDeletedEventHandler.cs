using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.GamePlan.Domain.Shared.Products;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Product
{
    public class BulkProductDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkProductDeleted>
    {
        private readonly IProductRepository _productRepository;

        public BulkProductDeletedEventHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public override void Handle(IBulkProductDeleted command)
        {
            _productRepository.DeleteRangeByExternalRefs(command.Data.Select(p => p.Externalidentifier));
            _productRepository.SaveChanges();
        }
    }
}
