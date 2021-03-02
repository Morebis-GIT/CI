using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects
{
    public class FaultType
    {
        public int Id { get; set; }

        public Dictionary<string, string> ShortName = new Dictionary<string, string>();

        public Dictionary<string, string> Description = new Dictionary<string, string>();

        public bool IsSelected { get; set; } = true;
    }
}
