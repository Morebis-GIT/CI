using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects.AgCampaigns;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers.Interfaces
{
    /// <summary>
    /// Serializes campaigns into the xml file.
    /// </summary>
    public interface ICampaignSerializer
    {
        /// <summary>Gets the filename.</summary>
        /// <value>The filename.</value>
        string Filename { get; }

        /// <summary>Serializes campaigns into the specified folder.</summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <param name="run">The run.</param>
        /// <param name="campaigns">The campaigns.</param>
        /// <param name="demographics">The demographics.</param>
        /// <param name="salesAreas">The sales areas.</param>
        /// <param name="programmeDictionaries">The programme dictionaries.</param>
        /// <param name="programmeCategories">The programme categories.</param>
        /// <param name="autoBookDefaultParameters">The automatic book default parameters.</param>
        /// <param name="channelGroups">The channel groups.</param>
        void Serialize(
            string folderName,
            Run run,
            IReadOnlyCollection<Campaign> campaigns,
            IReadOnlyCollection<BusinessType> businessTypes,
            IReadOnlyCollection<Demographic> demographics,
            IReadOnlyCollection<SalesArea> salesAreas,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            IReadOnlyCollection<ProgrammeCategoryHierarchy> programmeCategories,
            IAutoBookDefaultParameters autoBookDefaultParameters,
            out IReadOnlyCollection<Tuple<int, int, SalesAreaGroup>> channelGroups,
            out List<AgCampaignInclusion> campaignIncludeFunctions);
    }
}
