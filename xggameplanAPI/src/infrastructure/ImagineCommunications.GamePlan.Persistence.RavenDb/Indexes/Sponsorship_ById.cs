using System.Linq;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Objects;
using Raven.Client.Indexes;


namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
#pragma warning disable CA1707 // Identifiers should not contain underscores
    public class Sponsorship_ById
        : AbstractIndexCreationTask<Sponsorship>
    {
        public static string DefaultIndexName => "Sponsorship/ById";

        public Sponsorship_ById()
        {
            Map = sponsorships =>
                from sponsorship in sponsorships
                select new
                {
                    sponsorship.Id
                };
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
