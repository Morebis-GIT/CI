using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces
{
    public interface IProgTxDetailSerializer
    {
        /// <summary>Gets the filename.</summary>
        /// <value>The filename.</value>
        string Filename { get; }

        /// <summary>
        /// Serializes programme details into the specified folder name.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="programs">The list of programme.</param>
        /// <param name="programmeDictionaries">The programme dictionaries.</param>
        /// <param name="programmeCategories">The programme categories.</param>
        /// <param name="salesAreas">The sales areas.</param>
        /// <param name="autoBookDefaultParameters">The automatic book default parameters.</param>
        void Serialize(
            string folderName,
            IReadOnlyCollection<Programme> programs,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories,
            IReadOnlyCollection<SalesArea> salesAreas,
            IAutoBookDefaultParameters autoBookDefaultParameters);
    }
}
