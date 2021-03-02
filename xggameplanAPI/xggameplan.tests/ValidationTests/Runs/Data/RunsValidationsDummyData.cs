using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.tests.ValidationTests.Runs.Data
{
    public static class RunsValidationsDummyData
    {
        private static readonly Fixture _fixture = new Fixture();
        private const string _passName = "Test Pass";

        public static string PassName => _passName;
        public static int NumberOfItemsToCreate => 3;

        public static List<Scenario> GetScenarios() =>
            _fixture
                .CreateMany<Scenario>(NumberOfItemsToCreate)
                .ToList();

        public static Scenario GetScenario() =>
            _fixture.Build<Scenario>()
                .With(o => o.Passes, GetPassReferences())
                .With(o => o.CampaignPassPriorities, GetCampaignPassPriorities())
                .Create();

        public static List<CampaignPassPriority> GetCampaignPassPriorities()
        {
            return _fixture.Build<CampaignPassPriority>()
                .With(o => o.PassPriorities, _fixture.CreateMany<PassPriority>(NumberOfItemsToCreate).ToList())
                .CreateMany(NumberOfItemsToCreate).ToList();
        }

        public static List<PassReference> GetPassReferences() =>
            _fixture
                .CreateMany<PassReference>(NumberOfItemsToCreate)
                .ToList();

        public static TenantSettings GetTenantSettings() =>
            _fixture
                .Build<TenantSettings>()
                .With(o => o.MidnightStartTime, "240000")
                .With(o => o.MidnightEndTime, "995959")
                .With(o => o.PeakStartTime, "180000")
                .With(o => o.PeakEndTime, "220000")
                .Create();

        public static PassSalesAreaPriority GetPassSalesAreaPriorityWithFlagsTrue()
        {
            return _fixture.Build<PassSalesAreaPriority>()
                        .With(o => o.IsOffPeakTime, true)
                        .With(o => o.IsPeakTime, true)
                        .With(o => o.IsMidnightTime, true)
                        .With(o => o.SalesAreaPriorities, GetRunSalesAreaPriorities())
                        .With(o => o.StartDate, new DateTime(2020, 1, 2))
                        .With(o => o.EndDate, new DateTime(2020, 1, 5))
                        .With(o => o.StartTime, new TimeSpan(6, 0, 0))
                        .With(o => o.EndTime, new TimeSpan(5, 59, 59))
                        .With(o => o.DaysOfWeek, "1111111")
                        .Create();
        }

        public static PassSalesAreaPriority GetPassSalesAreaPriorityWithFlagsFalse()
        {
            return _fixture.Build<PassSalesAreaPriority>()
                        .With(o => o.IsOffPeakTime, false)
                        .With(o => o.IsPeakTime, false)
                        .With(o => o.IsMidnightTime, false)
                        .With(o => o.SalesAreaPriorities, GetRunSalesAreaPriorities())
                        .With(o => o.StartDate, new DateTime(2020, 1, 2))
                        .With(o => o.EndDate, new DateTime(2020, 1, 5))
                        .With(o => o.StartTime, new TimeSpan(6, 0, 0))
                        .With(o => o.EndTime, new TimeSpan(5, 59, 59))
                        .With(o => o.DaysOfWeek, "1111111")
                        .Create();
        }

        public static Run GetRun()
        {
            return _fixture.Build<Run>()
                .With(o => o.StartDate, new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .With(o => o.EndDate, new DateTime(2020, 2, 2, 0, 0, 0, DateTimeKind.Utc))
                .With(o => o.StartTime, new TimeSpan(6, 0, 0))
                .With(o => o.EndTime, new TimeSpan(5, 59, 59))
                .With(o => o.ISR, false)
                .With(o => o.Smooth, false)
                .With(o => o.Optimisation, false)
                .With(o => o.RightSizer, false)
                .With(o => o.NumberOfWeeks, 20)
                .With(o => o.EfficiencyPeriod, EfficiencyCalculationPeriod.NumberOfWeeks)
                .With(o => o.Scenarios, _fixture.Build<RunScenario>()
                                        .With(x => x.Status, ScenarioStatuses.Pending)
                                        .CreateMany(1).ToList())
                .With(o => o.SalesAreaPriorities, GetRunSalesAreaPriorities())
                .With(o => o.Campaigns, new List<CampaignReference>())
                .With(o => o.CampaignsProcessesSettings, new List<CampaignRunProcessesSettings>())
                .With(o => o.AnalysisGroupTargets, GetAnalysisGroupTargets())
                .Create();
        }

        public static Pass GetPass()
        {
            return _fixture.Build<Pass>()
                .With(o => o.Name, _passName)
                .With(o => o.PassSalesAreaPriorities, GetPassSalesAreaPriorityWithFlagsFalse())
                .With(o => o.General, _fixture.Build<General>()
                                        .With(x => x.Value, "8")
                                        .With(x => x.RuleId, (int)RuleID.MaximumRank)
                                        .CreateMany(NumberOfItemsToCreate).ToList())
                .With(o => o.BreakExclusions, new List<BreakExclusion>())
                .With(o => o.SlottingLimits, new List<SlottingLimit>())
                .Create();
        }

        public static List<SalesAreaPriority> GetRunSalesAreaPriorities() =>
            new List<SalesAreaPriority> {
                new SalesAreaPriority { SalesArea = "5 Select", Priority = SalesAreaPriorityType.Priority1 },
                new SalesAreaPriority { SalesArea = "5 USA", Priority = SalesAreaPriorityType.Priority2 },
                new SalesAreaPriority { SalesArea = "CBS Drama", Priority = SalesAreaPriorityType.Priority3 },
                new SalesAreaPriority { SalesArea = "Challenge", Priority = SalesAreaPriorityType.Exclude },
                new SalesAreaPriority { SalesArea = "Syfy", Priority = SalesAreaPriorityType.Exclude }
            };

        public static List<SalesArea> GetSalesAreas()
        {
            var salesAreaPriorities = GetRunSalesAreaPriorities();
            var salesAreas = _fixture.CreateMany<SalesArea>(salesAreaPriorities.Count).ToList();
            for (int i = 0; i < salesAreaPriorities.Count; i++)
            {
                salesAreas[i].Name = salesAreaPriorities[i].SalesArea;
            }
            return salesAreas;
        }

        public static List<AnalysisGroupNameModel> GetAnalysisGroups() =>
            new List<AnalysisGroupNameModel>
            {
                new AnalysisGroupNameModel {Id = 1, Name = "test"},
                new AnalysisGroupNameModel {Id = 2, Name = "test2"},
                new AnalysisGroupNameModel {Id = 3, Name = "test3"}
            };

        public static List<AnalysisGroupTarget> GetAnalysisGroupTargets()
        {
            var analysisGroups = GetAnalysisGroups();
            var analysisGroupTargets = _fixture.CreateMany<AnalysisGroupTarget>(analysisGroups.Count).ToList();

            for (int i = 0; i < analysisGroupTargets.Count; i++)
            {
                analysisGroupTargets[i].AnalysisGroupId = analysisGroups[i].Id;
                analysisGroupTargets[i].KPI = "test";
            }

            return analysisGroupTargets;
        }
    }
}
