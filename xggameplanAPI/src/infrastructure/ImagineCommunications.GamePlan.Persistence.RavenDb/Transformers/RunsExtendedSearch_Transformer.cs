using System;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class RunsExtendedSearch_Transformer
        : AbstractTransformerCreationTask<Run>
    {
        public RunsExtendedSearch_Transformer()
        {
            TransformResults = results =>
                from run in results
                let runId = run.Id.ToString()
                let scenarios = Include<Scenario>(run.Scenarios.Select(s => "scenarios/" + s.Id))
                let passes = scenarios.SelectMany(s => s.Passes)
                                      .Select(p => Include<Pass>("passes/" + p.Id))

                select new RunExtendedSearchModel
                {
                    Id = Guid.Parse(runId.Substring(runId.IndexOf('/') + 1)),
                    CustomId = run.CustomId,
                    Description = run.Description,
                    CreatedDateTime = run.CreatedDateTime,
                    StartDate = run.StartDate,
                    StartTime = run.StartTime,
                    EndDate = run.EndDate,
                    EndTime = run.EndTime,
                    LastModifiedDateTime = run.LastModifiedDateTime,
                    ExecuteStartedDateTime = run.ExecuteStartedDateTime,
                    IsLocked = run.IsLocked,
                    InventoryLock = run.InventoryLock,
                    Real = run.Real,
                    Smooth = run.Smooth,
                    SmoothDateRange = run.SmoothDateRange,
                    ISR = run.ISR,
                    ISRDateRange = run.ISRDateRange,
                    Optimisation = run.Optimisation,
                    OptimisationDateRange = run.OptimisationDateRange,
                    RightSizer = run.RightSizer,
                    RightSizerDateRange = run.RightSizerDateRange,
                    SpreadProgramming = run.SpreadProgramming,
                    SkipLockedBreaks = run.SkipLockedBreaks,
                    IgnorePremiumCategoryBreaks = run.IgnorePremiumCategoryBreaks,
                    ExcludeBankHolidays = run.ExcludeBankHolidays,
                    ExcludeSchoolHolidays = run.ExcludeSchoolHolidays,
                    Objectives = run.Objectives,
                    Author = run.Author,
                    SalesAreaPriorities = run.SalesAreaPriorities,
                    Campaigns = run.Campaigns,
                    CampaignsProcessesSettings = run.CampaignsProcessesSettings,
                    Scenarios = run.Scenarios,
                    ScenarioIds = scenarios.Select(s => s.Id.ToString()),
                    ScenarioNames = scenarios.Select(s => s.Name),
                    PassIds = passes.Select(p => p.Id.ToString(CultureInfo.InvariantCulture)),
                    PassNames = passes.Select(p => p.Name)
                };
        }
    }
}
