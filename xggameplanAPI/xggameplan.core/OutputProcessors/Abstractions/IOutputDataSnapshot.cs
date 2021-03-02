using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace xggameplan.OutputProcessors.Abstractions
{
    public interface IOutputDataSnapshot
    {
        Lazy<Run> Run { get; }
        Lazy<Scenario> Scenario { get; }
        Lazy<IEnumerable<Pass>> ScenarioPasses { get; }
        Lazy<IEnumerable<BreakSimple>> BreaksForRun { get; }
        Lazy<IEnumerable<Spot>> SpotsForRun { get; }
        Lazy<IEnumerable<Campaign>> AllCampaigns { get; }
        Lazy<IEnumerable<SalesArea>> AllSalesAreas { get; }
        Lazy<IEnumerable<Demographic>> AllDemographics { get; }
        Lazy<IEnumerable<ProgrammeDictionary>> AllProgrammeDictionaries { get; }
        Lazy<IEnumerable<string>> BreakTypes { get; }
    }
}
