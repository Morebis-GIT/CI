using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class FunctionalAreaFaultTypes_Transformer : AbstractTransformerCreationTask<FunctionalArea>
    {
        public class Result
        {
            public string Id { get; set; }

            public Guid FunctionalAreaId => new Guid(Id.Split('/')[1]);

            public int FaultTypeId { get; set; }

            public string FaultTypeDesc { get; set; }

            public Dictionary<string, string> Description = new Dictionary<string, string>();

            public bool IsSelected { get; set; }
        }

        public FunctionalAreaFaultTypes_Transformer()
        {
            TransformResults = functionalAreas =>
                from functionalArea in functionalAreas
                from faultType in functionalArea.FaultTypes
                select new
                {
                    Id = functionalArea.Id,
                    FaultTypeId = faultType.Id,
                    Description = faultType.Description,
                    IsSelected = faultType.IsSelected
                };
        }
    }
}
