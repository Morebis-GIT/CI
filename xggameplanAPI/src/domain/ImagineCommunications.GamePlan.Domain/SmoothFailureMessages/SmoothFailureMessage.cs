using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothFailureMessages
{
    public class SmoothFailureMessage
    {
        public int Id { get; set; }

        public Dictionary<string, string> Description = new Dictionary<string, string>();   // Multi-language
    }
}
