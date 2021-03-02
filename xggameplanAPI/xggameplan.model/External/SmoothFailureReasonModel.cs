using System.Collections.Generic;

namespace xggameplan.Model
{
    public class SmoothFailureMessageModel
    {
        public int Id { get; set; }

        public Dictionary<string, string> Description = new Dictionary<string, string>();   // Multi-language
    }
}
