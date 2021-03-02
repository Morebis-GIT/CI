using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects
{
    public class FunctionalArea
    {
        public Guid Id { get; set; }

        public Dictionary<string, string> Description = new Dictionary<string, string>();

        public List<FaultType> FaultTypes = new List<FaultType>();
    }
}
