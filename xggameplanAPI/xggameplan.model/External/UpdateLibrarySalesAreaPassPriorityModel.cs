using System;
using Newtonsoft.Json;

namespace xggameplan.Model
{
    public class UpdateLibrarySalesAreaPassPriorityModel : LibrarySalesAreaPassPriorityModelBase
    {
        [JsonIgnore]
        public Guid Uid { get; set; }
    }
}
