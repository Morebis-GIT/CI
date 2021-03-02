using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class BulkProductCreatedOrUpdated : IBulkProductCreatedOrUpdated
    {
        public IEnumerable<IProductCreatedOrUpdated> Data { get; }

        public BulkProductCreatedOrUpdated(IEnumerable<ProductCreatedOrUpdated> data) => Data = data;
    }
}
