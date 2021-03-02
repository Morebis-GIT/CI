using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Programmes_ByIdAndSalesAreaStartDateTime
        : AbstractIndexCreationTask<Programme, Programmes_ByIdAndSalesAreaStartDateTime.IndexedFields>
    {
        public static string DefaultIndexName => "Programmes/ByIdAndSalesAreaStartDateTime";

        public class IndexedFields
        {
            public Guid Id { get; set; }

            public string TokenizedProgramme { get; set; }

            public string SalesArea { get; set; }

            public DateTime StartDateTime { get; set; }

            public string ExternalReference { get; set; }

            public string ProgrammeName { get; set; }
        }

        public Programmes_ByIdAndSalesAreaStartDateTime()
        {
            Map = programmes =>
                from programme in programmes
                select new
                {
                    programme.Id,
                    programme.SalesArea,
                    programme.StartDateTime,
                    TokenizedProgramme = $"{programme.ExternalReference} {programme.ProgrammeName}",
                    programme.ExternalReference,
                    programme.ProgrammeName
                };

            Index(p => p.TokenizedProgramme, FieldIndexing.Analyzed);
        }
    }
}
