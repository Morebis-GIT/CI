using System.Linq;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class SpotPlacements_Default : AbstractIndexCreationTask<SpotPlacement>
    {
        public static string DefaultIndexName => "SpotPlacements/Default";

        public SpotPlacements_Default()
        {
            Map = spotPlacements => from spotPlacement in spotPlacements
                                    select new
                                    {
                                        spotPlacement.ExternalSpotRef,
                                        spotPlacement.ModifiedTime
                                    };
        }
    }
}
