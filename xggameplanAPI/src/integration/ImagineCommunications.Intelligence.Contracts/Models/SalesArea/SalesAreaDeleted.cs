using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea
{
    public class SalesAreaDeleted : ISalesAreaDeleted
    {
        public SalesAreaDeleted(string shortName) => ShortName = shortName;

        public string ShortName { get; }
    }
}
