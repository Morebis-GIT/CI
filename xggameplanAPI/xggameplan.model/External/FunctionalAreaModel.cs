using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class FunctionalAreaModel
    {
        public Guid Id { get; set; }

        public Dictionary<string, string> Description = new Dictionary<string, string>();

        public List<FaultTypeModel> FaultTypes = new List<FaultTypeModel>();
    }


}
