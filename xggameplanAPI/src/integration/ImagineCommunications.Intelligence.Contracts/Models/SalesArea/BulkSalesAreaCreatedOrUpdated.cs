using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea
{
    public class BulkSalesAreaCreatedOrUpdated : IBulkSalesAreaCreatedOrUpdated
    {
        public IEnumerable<ISalesAreaCreatedOrUpdated> Data { get; }

        public BulkSalesAreaCreatedOrUpdated(IEnumerable<SalesAreaCreatedOrUpdated> data) => Data = data;
    }
}
