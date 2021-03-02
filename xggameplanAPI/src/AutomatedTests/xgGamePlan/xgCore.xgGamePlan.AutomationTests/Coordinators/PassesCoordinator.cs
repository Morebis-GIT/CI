using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Coordinators
{
    [Binding]
    public class PassesCoordinator
    {
        private readonly IPassesApi _passesApi;
        private readonly IFixture _fixture;

        public PassesCoordinator(IPassesApi passesApi, IFixture fixture)
        {
            _passesApi = passesApi;
            _fixture = fixture;
        }

        /// <summary>
        /// Creates some number of passes
        /// </summary>
        /// <param name="count"></param>
        /// <param name="isLibraried">Should be true when is not linked to any scenario, otherwise false</param>
        /// <returns></returns>
        public IEnumerable<Pass> BuildPasses(int count, bool isLibraried)
        {
            #region Set General, Weightings, Tolerances, Rules, PassSalesAreaPriorities

            var general = new List<GeneralModel>()
            {
                new GeneralModel
                {
                    RuleId = 1,
                    InternalType = "Defaults",
                    Description = "Minimum Efficiency",
                    Value = "11",
                    Type = "general"
                }
            };

            var weightings = new List<WeightingModel>()
            {
                new WeightingModel
                {
                    RuleId = 1,
                    InternalType = "Weightings",
                    Description = "Campaign",
                    Value = "1",
                    Type = "weightings"
                }
            };

            var tolerances = new List<ToleranceModel>()
            {
                new ToleranceModel
                {
                    RuleId = 1,
                    InternalType = "Campaign",
                    Description = "Campaign",
                    Value = null,
                    Under = 0,
                    Over = 10,
                    Ignore = true,
                    ForceOverUnder = ForceOverUnder.Over,
                    Type = "tolerances"
                }
            };

            var rules = new List<RuleModel>()
            {
                new RuleModel
                {
                    RuleId = 1,
                    InternalType = "Slotting Controls",
                    Description = "Max Spots Per Day",
                    Value = "15",
                    PeakValue = null,
                    Type = "rules"
                }
            };

            var passSalesAreaPriorities = new PassSalesAreaPriority()
            {
                SalesAreaPriorities = new List<SalesAreaPriorityModel>(),
                StartDate = null,
                EndDate = null,
                StartTime = null,
                EndTime = null,
                DaysOfWeek = null
            };

            #endregion

            return _fixture.Build<Pass>()
                .Without(p => p.Id)
                .With(p => p.General, general)
                .With(p => p.Weightings, weightings)
                .With(p => p.Tolerances, tolerances)
                .With(p => p.Rules, rules)
                .With(p => p.PassSalesAreaPriorities, passSalesAreaPriorities)
                .With(p => p.BreakExclusions, new List<BreakExclusionModel>())
                .With(p => p.ProgrammeRepetitions, new List<ProgrammeRepetitionModel>())
                .With(p => p.SlottingLimits, new List<SlottingLimitModel>())
                .With(p => p.DateCreated, DateTime.UtcNow)
                .With(p => p.DateModified, DateTime.UtcNow)
                .With(p => p.IsLibraried, isLibraried)
                .CreateMany(count);
        }

        public async Task<IEnumerable<Pass>> CreatePassesAsync(int count)
        {
            var passes = BuildPasses(count, true);
            var res = new List<Pass>();
            foreach (var pass in passes)
            {
                res.Add(await _passesApi.Create(pass).ConfigureAwait(false));
            }

            return res;
        }
    }
}
