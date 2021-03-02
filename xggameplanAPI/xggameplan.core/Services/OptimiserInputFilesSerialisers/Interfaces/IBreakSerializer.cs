using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces
{
    /// <summary>
    /// Serializes breaks into the xml file.
    /// </summary>
    public interface IBreakSerializer
    {
        /// <summary>Gets the filename.</summary>
        /// <value>The filename.</value>
        string Filename { get; }

        /// <summary>Serializes breaks into the specified folder.</summary>
        /// <param name="folderName">Name of the folder.</param>
        void Serialize(
            string folderName,
            Run run,
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<Demographic> demographics,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories,
            IReadOnlyCollection<Programme> scheduleProgrammes,
            IReadOnlyCollection<Campaign> campaignWithProgrammeRestrictions,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            out IReadOnlyCollection<BreakWithProgramme> breaksWithProgrammes,
            out IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries);
    }
}
