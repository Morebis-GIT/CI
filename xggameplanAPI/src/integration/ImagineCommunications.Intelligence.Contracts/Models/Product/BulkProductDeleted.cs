using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class BulkProductDeleted : IBulkProductDeleted
    {
        public IEnumerable<IProductDeleted> Data { get; }

        public BulkProductDeleted(IEnumerable<ProductDeleted> data) => Data = data;
    }
}
