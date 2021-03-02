using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ScenarioFailures.Objects
{
    public class Failures
    {
        public Guid Id { get; set; }
        public List<Failure> Items = new List<Failure>();
    }
}
