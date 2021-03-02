using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT14638
{
    internal class UpdateStepXGGT14638_AddFiveNewWeightingRules : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        private readonly List<(int ruleId, string name)> _newRules = new List<(int ruleId, string name)>
        {
            (10, "Strike Weight/Daypart"),
            (11, "Strike Weight/Length"),
            (12, "Strike Weight/Daypart/Length"),
            (13, "Campaign Price"),
            (14, "Programme")
        };

        public UpdateStepXGGT14638_AddFiveNewWeightingRules(IEnumerable<string> tenantConnectionStrings) =>
            _tenantConnectionStrings = tenantConnectionStrings;

        public Guid Id => new Guid("69055089-2463-403C-9BC0-A01D557EF5E4");

        public int Sequence => 1;

        public string Name => "XGGT-14638";

        public bool SupportsRollback => false;

        public void Apply()
        {
            foreach (var connectionString in _tenantConnectionStrings)
            {
                using (var documentStore = DocumentStoreFactory.CreateStore(connectionString, null))
                using (var session = documentStore.OpenSession())
                {
                    InsertRules(session);
                    InsertPassRules(session);
                    session.SaveChanges();
                }
            }
        }

        public void RollBack() => throw new NotImplementedException();

        private void InsertRules(IDocumentSession session)
        {
            var weightingRuleNames = session.Query<Rule>()
                .Where(rule => rule.Type == "weightings")
                .Select(rule => rule.Description)
                .ToHashSet();

            foreach (var (ruleId, ruleName) in _newRules)
            {
                if (!weightingRuleNames.Contains(ruleName))
                {
                    session.Store(new Rule
                    {
                        RuleId = ruleId,
                        RuleTypeId = 2,
                        InternalType = "Weightings",
                        Description = ruleName,
                        Type = "weightings"
                    });
                }
            }
        }

        private void InsertPassRules(IDocumentSession session)
        {
            IScenarioRepository scenarioRepository = new RavenScenarioRepository(session);
            IPassRepository passRepository = new RavenPassRepository(session);

            var librariedScenarios = scenarioRepository.GetAll()
                .Where(scenario => scenario.IsLibraried == true)
                .ToList();

            var librariedScenariosPassIds = librariedScenarios
                .SelectMany(scenario => scenario.Passes)
                .Select(passRef => passRef.Id)
                .ToHashSet();

            var librariedPasses = passRepository.GetAll()
                .Where(pass => pass.IsLibraried == true
                            || librariedScenariosPassIds.Contains(pass.Id))
                .ToList();

            foreach (var pass in librariedPasses)
            {
                var weightingNames = pass.Weightings
                    .Select(w => w.Description)
                    .ToHashSet();

                bool passUpdated = false;

                foreach (var (ruleId, ruleName) in _newRules)
                {
                    if (!weightingNames.Contains(ruleName))
                    {
                        pass.Weightings.Add(new Weighting
                        {
                            RuleId = ruleId,
                            InternalType = "Weightings",
                            Description = ruleName,
                            Value = "0",
                            Type = "weightings"
                        });

                        passUpdated = true;
                    }
                }

                if (passUpdated)
                {
                    session.Store(pass);
                }
            }
        }
    }
}
