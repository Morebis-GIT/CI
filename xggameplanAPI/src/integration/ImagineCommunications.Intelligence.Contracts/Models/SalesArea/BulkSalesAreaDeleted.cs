using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea
{
    public class BulkSalesAreaDeleted : IBulkSalesAreaDeleted
    {
        public IEnumerable<ISalesAreaDeleted> Data { get; }

        public BulkSalesAreaDeleted(IEnumerable<SalesAreaDeleted> data) => Data = data;
    }
}
