using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class ScenarioDigestListItemModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateUserModified { get; set; }
        public bool IsDefault { get; set; }
        public List<PassDigestListItemModel> Passes { get; set; }
    }
}
