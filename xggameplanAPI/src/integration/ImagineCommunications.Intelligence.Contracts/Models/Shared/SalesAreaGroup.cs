using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class SalesAreaGroup
    {
        public SalesAreaGroup(string groupName, List<string> salesAreas)
        {
            GroupName = groupName;
            SalesAreas = salesAreas;
        }

        public string GroupName { get; }

        public List<string> SalesAreas { get; }
    }
}
