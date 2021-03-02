using System.Linq;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using Raven.Client.Indexes;

#pragma warning disable CA1707 // Identifiers should not contain underscores
namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class ProgrammeDictionaries_ByExternalReferenceAndId
        : AbstractIndexCreationTask<ProgrammeDictionary>
    {
        public static string DefaultIndexName => "ProgrammeDictionaries/ByExternalReferenceAndId";

        public ProgrammeDictionaries_ByExternalReferenceAndId()
        {
            Map = programmeDictionaries =>
                from programmeDictionary in programmeDictionaries
                select new
                {
                    programmeDictionary.ExternalReference,
                    programmeDictionary.ProgrammeName,
                    programmeDictionary.Id
                };
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
