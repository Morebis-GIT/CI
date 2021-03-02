using System.Collections.Generic;

namespace xggameplan.Model
{
    public class FaultTypeModel
    {
        public int Id { get; set; }

        public Dictionary<string, string> Description = new Dictionary<string, string>();

        public bool IsSelected { get; set; } = true;
    }

}
